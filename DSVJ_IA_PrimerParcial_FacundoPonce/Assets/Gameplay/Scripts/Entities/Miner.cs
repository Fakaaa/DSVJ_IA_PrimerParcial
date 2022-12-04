using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using PrimerParcial.Gameplay.Entities.Agent;
using PrimerParcial.Gameplay.Entities;
using PrimerParcial.Gameplay.Controllers;
using PrimerParcial.Gameplay.Interfaces;
using PrimerParcial.Gameplay.Voronoi.Utils;

public class Miner : MonoBehaviour
{
    #region ENUMS
    public enum States
    {
        Mining,
        GoToMine,
        GoToUrbanCenter,
        GoToHide,
        Idle,

        _Count
    }

    public enum Flags
    {
        OnFullInventory,
        OnReachMine,
        OnReachUrbanCenter,
        OnEmpyMine,
        Alert,

        _Count
    }
    #endregion

    #region EXPOSED_FIELDS
    [SerializeField] private Animator minerAnim = null;
    [SerializeField] private List<Line> minesConnections = new List<Line>();
    [SerializeField] private List<Line> mediatrices = new List<Line>();
    #endregion

    #region PRIVATE_FIELDS
    private float timeUntilGoUrbanCenter = 4f;
    private float timer = 0f;
    private bool firstMovement = false;
    private bool isGoingToTarget = false;
    private bool isOnShelter = false;
    
    private Vector2 lastMinePosition;
    private AgentFSM minerBehaviour = null;

    private UrbanCenter urbanCenter = null;
    private Mine actualTargetMine = null;

    private Vector2 currentNodePosition = default;
    private List<Mine> allMinesOnMap = null;
    private List<Vector2> minerPath = new List<Vector2>();

    private Func<Vector2, Vector2 ,List<Vector2>> onGetPathToMine = null;
    private Func<Vector2, Mine> onGetClosestMine = null;
    #endregion

    #region PROPERTIES
    public Func<Vector2, Vector2 ,List<Vector2>> OnGetPathOnMap { get { return onGetPathToMine; } set { onGetPathToMine = value; } }
    #endregion

    #region PUBLIC_METHODS
    public void Init(List<Mine> minesOnMap, UrbanCenter urbanCenter, Func<Vector2,Mine> onGetClosestMine)
	{
        this.urbanCenter = urbanCenter;
        this.onGetClosestMine = onGetClosestMine;
        
        if(minerPath != null)
        {
            minerPath.Clear();
        }
        else
        {
            minerPath = new List<Vector2>();
        }

        allMinesOnMap = minesOnMap;

        minerBehaviour = new AgentFSM((int)States._Count, (int)Flags._Count);

        firstMovement = false;
        isOnShelter = false;
        
        InitializeMinerFSM();
        FindClosestMine();

        currentNodePosition = transform.position;
    }

    public void UpdateMiner()
    {
        minerBehaviour.Update();
    }

    public void ToggleBehaviour()
    {
        minerBehaviour.ToggleBehaviour(true);
    }

    public void DestroyMiner()
    {
        minerBehaviour.ToggleBehaviour(false);

        StopAllCoroutines();

        if(minerPath != null)
        {
            minerPath.Clear();
        }

        Destroy(gameObject, 0.15f);
    }

    public void EscapeFromDanger()
    {
        if(minerBehaviour.GetCurrentState() == (int)States.GoToHide)
            return;
        
        StopAllCoroutines();
        isGoingToTarget = false;
        firstMovement = true;

        minerBehaviour.SetFlag((int)Flags.Alert);
    }
    
    public void ContinueWork()
    {
        StopAllCoroutines();

        isOnShelter = false;        
        isGoingToTarget = false;

        minerBehaviour.SetFlag((int)Flags.OnReachUrbanCenter);
    }
    #endregion

    #region PRIVATE_METHODS
    private void InitializeMinerFSM()
    {
        minerBehaviour.ForceCurretState((int)States.GoToMine);
        minerBehaviour.SetRelation((int)States.GoToMine, (int)Flags.OnReachMine, (int)States.Mining);
        minerBehaviour.SetRelation((int)States.Mining, (int)Flags.OnFullInventory, (int)States.GoToUrbanCenter);
        minerBehaviour.SetRelation((int)States.GoToUrbanCenter, (int)Flags.OnReachUrbanCenter, (int)States.GoToMine);
        minerBehaviour.SetRelation((int)States.GoToUrbanCenter, (int)Flags.OnEmpyMine, (int)States.Idle);
        
        minerBehaviour.SetRelation((int)States.Idle, (int)Flags.Alert, (int)States.GoToHide);
        minerBehaviour.SetRelation((int)States.Mining, (int)Flags.Alert, (int)States.GoToHide);
        minerBehaviour.SetRelation((int)States.GoToMine, (int)Flags.Alert, (int)States.GoToHide);
        minerBehaviour.SetRelation((int)States.GoToUrbanCenter, (int)Flags.Alert, (int)States.GoToHide);
        
        minerBehaviour.SetRelation((int)States.GoToHide, (int)Flags.OnReachUrbanCenter, (int)States.GoToMine);
        
        ConfigureMinerBehaviours();
    }

    private void ConfigureMinerBehaviours()
    {
        minerBehaviour.AddBehaviour((int)States.Idle, Idle);
        minerBehaviour.AddBehaviour((int)States.Mining, Mining);
        minerBehaviour.AddBehaviour((int)States.GoToMine, GoToMine);
        minerBehaviour.AddBehaviour((int)States.GoToUrbanCenter, GoToUrbanCenter);
        minerBehaviour.AddBehaviour((int)States.GoToHide, EscapeToUrbanCenter);
    }

    #region MINER_UTILS
    private void FindClosestMine()
    {
        if(actualTargetMine != null)
        {
            return;
        }

        Mine closestMine = onGetClosestMine?.Invoke(transform.position);

        actualTargetMine = closestMine;
    }
    #endregion

    #region MINER_BEHAVIOURS
    private void Idle()
    {
        minerAnim.SetBool("IsMining", false);
        minerAnim.SetBool("IsMoving", false);
    }

    private void Mining()
    {
        isGoingToTarget = false;
        minerAnim.SetBool("IsMoving", false);
        minerAnim.SetBool("IsMining", true);

        if(timer < timeUntilGoUrbanCenter)
        {
            timer += Time.deltaTime;

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);

            foreach (Collider2D hit in hits)
            {
                if(hit.TryGetComponent(out IMinable mine))
                {
                    if(mine.OnMine(UnityEngine.Random.Range(0,3)))
                    {
                        Debug.Log("Succed mine!");
                    }
                    else
                    {
                        Debug.Log("La mine se quedo vacia bruh");
                        actualTargetMine = null;
                        timer = 0;
                        minerBehaviour.SetFlag((int)Flags.OnFullInventory);
                        StopAllCoroutines();
                    }
                }
                else
                {
                    Debug.Log("La mine se quedo vacia bruh");
                    timer = 0;
                    actualTargetMine = null;
                    minerBehaviour.SetFlag((int)Flags.OnFullInventory);
                    StopAllCoroutines();
                }
            }

            if (hits.Length < 1)
            {
                Debug.Log("La mine se quedo vacia bruh");
                timer = 0;
                actualTargetMine = null;
                minerBehaviour.SetFlag((int)Flags.OnFullInventory);
                StopAllCoroutines();
            }
        }
        else
        {
            timer = 0f;
            minerBehaviour.SetFlag((int)Flags.OnFullInventory);
        }
    }

    private void GoToMine()
    {
        if (actualTargetMine == null && firstMovement)
        {
            return;
        }

        if(isGoingToTarget)
        {
            return;
        }

        minerAnim.SetBool("IsMining", false);
        minerAnim.SetBool("IsMoving", true);

        if(minerPath != null)
        {
            minerPath.Clear();
        }

        if(actualTargetMine != null)
        {
            actualTargetMine = actualTargetMine.IsEmpty ? null : actualTargetMine;
        }
        
        if(actualTargetMine != null)
        {
            //Vector2 minerPosition = transform.position;
            //Vector2 originPosition = default;
//
            //if (!firstMovement)
            //    originPosition = minerPosition;
            //else
            //    originPosition = urbanCenter.attachedNode.GetCellPosition();
            
            minerPath = OnGetPathOnMap?.Invoke(currentNodePosition, actualTargetMine.GetMinePosition());
        }

        StartCoroutine(MoveMinerToDestination((state) =>
        {
            firstMovement = true;
            lastMinePosition = actualTargetMine.Position;
            minerBehaviour.SetFlag((int)Flags.OnReachMine);
        }));

        UpdateAllMinesListState();
    }

    private void UpdateAllMinesListState()
    {
        List<Mine> toRemoveMines = allMinesOnMap.Where(mine => mine == null).ToList();

        for (int i = 0; i < toRemoveMines.Count; i++)
        {
            allMinesOnMap.Remove(toRemoveMines[i]);
        }
    }

    private void EscapeToUrbanCenter()
    {
        if (isGoingToTarget || isOnShelter)
        {
            minerAnim.SetBool("IsMining", false);
            minerAnim.SetBool("IsMoving", false);
            return;
        }
        
        minerAnim.SetBool("IsMining", false);
        minerAnim.SetBool("IsMoving", true);

        if (minerPath != null)
        {
            minerPath.Clear();
        }

        minerPath = OnGetPathOnMap?.Invoke(currentNodePosition, urbanCenter.attachedNode.GetCellPosition());
        
        StartCoroutine(MoveMinerToDestination((state) =>
        {
            isOnShelter = true;
            currentNodePosition = urbanCenter.attachedNode.GetCellPosition();
            minerBehaviour.SetFlag((int)Flags.OnEmpyMine);
        }));
    }
    
    private void GoToUrbanCenter()
    {
        if(isGoingToTarget)
        {
            return;
        }

        minerAnim.SetBool("IsMining", false);
        minerAnim.SetBool("IsMoving", true);

        if (minerPath != null)
        {
            
            minerPath.Clear();
        }

        minerPath = OnGetPathOnMap?.Invoke(lastMinePosition, urbanCenter.attachedNode.GetCellPosition());
        
        StartCoroutine(MoveMinerToDestination((state) =>
        {
            if (state)
            {
                minerBehaviour.SetFlag((int)Flags.OnReachUrbanCenter);
            }
            else
            {
                minerBehaviour.SetFlag((int)Flags.OnEmpyMine);
            }

            FindClosestMine();
        }));
    }
    #endregion

    #endregion

    #region CORUTINES
    IEnumerator MoveMinerToDestination(Action<bool> onReachDestination = null)
    {
        if(minerPath == null)
        {
            yield break;
        }

        if (!minerBehaviour.IsEnable)
        {
            yield break;
        }

        isGoingToTarget = true;

        
        if (minerPath.Any())
        {
            currentNodePosition = minerPath[0];
            
            foreach (Vector2 position in minerPath)
            {
                if(!minerBehaviour.IsEnable)
                {
                    yield break;
                }

                currentNodePosition = position;
                    
                while (Vector2.Distance(transform.position, position) > 0.05f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, position, (minerPath.Count *0.5f) * Time.deltaTime);

                    currentNodePosition = position;
                        
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        onReachDestination?.Invoke(allMinesOnMap.Count > 0);
        isGoingToTarget = false;
    }
    #endregion
}
