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
        Idle,

        _Count
    }

    public enum Flags
    {
        OnFullInventory,
        OnReachMine,
        OnReachUrbanCenter,
        OnEmpyMine,

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

    private AgentFSM minerBehaviour = null;

    private bool isGoingToTarget = false;

    private UrbanCenter urbanCenter = null;

    private Mine actualTargetMine = null;

    private List<Mine> allMinesOnMap = null;

    private List<Vector2Int> minerPath = new List<Vector2Int>();

    private Func<Vector2Int, Vector2Int ,List<Vector2Int>> onGetPathToMine = null;
    #endregion

    #region PROPERTIES
    public Func<Vector2Int, Vector2Int ,List<Vector2Int>> OnGetPathOnMap { get { return onGetPathToMine; } set { onGetPathToMine = value; } }
    #endregion

    #region PUBLIC_METHODS
    public void Init(List<Mine> minesOnMap, UrbanCenter urbanCenter)
	{
        this.urbanCenter = urbanCenter;

        if(minerPath != null)
        {
            minerPath.Clear();
        }
        else
        {
            minerPath = new List<Vector2Int>();
        }

        allMinesOnMap = minesOnMap;

        minerBehaviour = new AgentFSM((int)States._Count, (int)Flags._Count);

        InitializeMinerFSM();
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
    #endregion

    #region PRIVATE_METHODS
    private void InitializeMinerFSM()
    {
        minerBehaviour.ForceCurretState((int)States.GoToMine);
        minerBehaviour.SetRelation((int)States.GoToMine, (int)Flags.OnReachMine, (int)States.Mining);
        minerBehaviour.SetRelation((int)States.Mining, (int)Flags.OnFullInventory, (int)States.GoToUrbanCenter);
        minerBehaviour.SetRelation((int)States.GoToUrbanCenter, (int)Flags.OnReachUrbanCenter, (int)States.GoToMine);
        minerBehaviour.SetRelation((int)States.GoToUrbanCenter, (int)Flags.OnEmpyMine, (int)States.Idle);

        ConfigureMinerBehaviours();
    }

    private void ConfigureMinerBehaviours()
    {
        minerBehaviour.AddBehaviour((int)States.Idle, Idle);
        minerBehaviour.AddBehaviour((int)States.Mining, Mining);
        minerBehaviour.AddBehaviour((int)States.GoToMine, GoToMine);
        minerBehaviour.AddBehaviour((int)States.GoToUrbanCenter, GoToUrbanCenter);
    }

    #region MINER_UTILS
    private void FindClosestMine()
    {
        if(actualTargetMine != null)
        {
            return;
        }
        //This will make the Thiessen calc to find the nearest mine, but for now i will do something easy.
        Mine closestMine = null;
        if(allMinesOnMap.Count > 0)
        {
            closestMine = allMinesOnMap[0];
            float closestDistance = Vector2.Distance(transform.position, allMinesOnMap[0].GetMinePosition());
            for (int i = 1; i < allMinesOnMap.Count; i++)
            {
                if (allMinesOnMap[i] != null && !allMinesOnMap[i].IsEmpty)
                {
                    if(Vector2.Distance(transform.position, allMinesOnMap[i].GetMinePosition()) < closestDistance)
                    {
                        closestMine = allMinesOnMap[i];
                        closestDistance = Vector2.Distance(transform.position, allMinesOnMap[i].GetMinePosition());
                    }
                }
            }
        }

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

        FindClosestMine();

        if(actualTargetMine != null)
        {
            minerPath = OnGetPathOnMap?.Invoke(Vector2Int.RoundToInt(transform.position), actualTargetMine.GetMinePosition());
        }

        StartCoroutine(MoveMinerToDestination((state) => 
        {
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

        minerPath = OnGetPathOnMap?.Invoke(Vector2Int.RoundToInt(transform.position), urbanCenter.attachedNode.GetCellPosition());

        StartCoroutine(MoveMinerToDestination((state) => 
        {
            if(state)
            {
                minerBehaviour.SetFlag((int)Flags.OnReachUrbanCenter);
            }
            else
            {
                minerBehaviour.SetFlag((int)Flags.OnEmpyMine);
            }
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
            if(minerPath != null)
            {
                foreach (Vector2Int position in minerPath)
                {
                    if(!minerBehaviour.IsEnable)
                    {
                        yield break;
                    }

                    if(actualTargetMine == null)
                    {
                        onReachDestination?.Invoke(allMinesOnMap.Count > 0);
                        isGoingToTarget = false;
                        yield break;
                    }

                    while (Vector2.Distance(transform.position, position) > 0.15f)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, position, (minerPath.Count *0.5f) * Time.deltaTime);

                        yield return new WaitForEndOfFrame();
                    }
                }
            }
        }

        onReachDestination?.Invoke(allMinesOnMap.Count > 0);
        isGoingToTarget = false;

        yield break;
    }
    #endregion
}
