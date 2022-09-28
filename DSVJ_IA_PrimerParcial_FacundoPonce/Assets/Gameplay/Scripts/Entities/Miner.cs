using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using PrimerParcial.Gameplay.Entities.Agent;
using PrimerParcial.Gameplay.Entities;
using PrimerParcial.Gameplay.Controllers;

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

        Mine closestMine = allMinesOnMap[0];
        float closestDistance = Vector2.Distance(transform.position, allMinesOnMap[0].GetMinePosition());
        for (int i = 1; i < allMinesOnMap.Count; i++)
        {
            if (allMinesOnMap[i] != null)
            {
                if(Vector2.Distance(transform.position, allMinesOnMap[i].GetMinePosition()) < closestDistance)
                {
                    closestMine = allMinesOnMap[i];
                    closestDistance = Vector2.Distance(transform.position, allMinesOnMap[i].GetMinePosition());
                }
            }
        }

        actualTargetMine = closestMine;
    }
    #endregion

    #region MINER_BEHAVIOURS
    private void Idle()
    {
        Debug.Log("Miner is Idle");

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

        FindClosestMine();

        Debug.Log("GIVE ME A PATH");
        minerPath = OnGetPathOnMap?.Invoke(Vector2Int.RoundToInt(transform.position), actualTargetMine.GetMinePosition());

        StartCoroutine(MoveMinerToDestination(() => 
        {
            minerBehaviour.SetFlag((int)Flags.OnReachMine);
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

        Debug.Log("GIVE ME A PATH");
        minerPath = OnGetPathOnMap?.Invoke(Vector2Int.RoundToInt(transform.position), urbanCenter.attachedNode.GetCellPosition());

        StartCoroutine(MoveMinerToDestination(() => 
        {
            minerBehaviour.SetFlag((int)Flags.OnReachUrbanCenter);
        }));
    }
    #endregion

    #endregion

    #region CORUTINES
    IEnumerator MoveMinerToDestination(Action onReachDestination = null)
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
            foreach (Vector2Int position in minerPath)
            {
                if(!minerBehaviour.IsEnable)
                {
                    yield break;
                }

                while (Vector2.Distance(transform.position, position) > 0.15f)
                {
                    transform.position = Vector2.MoveTowards(transform.position, position, 1.15f * Time.deltaTime);

                    yield return new WaitForEndOfFrame();
                }
            }
        }

        onReachDestination?.Invoke();
        isGoingToTarget = false;

        yield break;
    }
    #endregion
}
