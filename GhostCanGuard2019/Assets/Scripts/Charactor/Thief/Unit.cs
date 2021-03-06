﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //** Values should be changed in the inspector**

    public Transform treasure, exit;// positions of treasure and exit
    public float initialSpeed = 5;// initial speed for thief
    public float increasedSpeed = 30, increasedSpeedDur = 3.0f;//increased speed and duration
    //public float stunnedTime = 1.5f;// stunned gimick effect duration

    Vector3[] path;
    int targetIndex;
    float currSpeed;// current speed

    public List<Transform> escapePoints = new List<Transform>();
    IOrderedEnumerable<Transform> escapePointsByDistance;
    //public List<Transform> prirityPoints = new List<Transform>();

    public Transform currTarget;// current target to head to

    Vector3 currentWayPoint;

    Thief thief;

    private float nextActionTime = 0.0f;
    public float period = 0.1f;

    //public bool mIsAllowFollow = false;
    //bool mIsHeadable = false;

    int pathFindIndex = 0;
    IEnumerator FollowPathCorotine;

    private void Awake()
    {
        StopAllCoroutines();
    }


    void Start()
    {
        //FollowPathCorotine = FollowPath();
        currTarget = treasure;
        currSpeed = initialSpeed;
        //InvokeRepeating("RefindPath", 0f, 0.5f);
        thief = GetComponent<Thief>();
        //mIsAllowFollow = true;
        //PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound);
        SortEscapePoints();
    }

    void Update()
    {
        //call this when
        //assuming path changed && after updating grid Gizmos
        //if (Input.GetKeyDown(KeyCode.A))// start pathfiding
        //{
        //    PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound);
        //}
        //if (Input.GetKeyDown(KeyCode.Q)) GotStunned();//stunned gimmick, 
        //PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound);
        if (thief.thiefState == Thief.ThiefState.GAMEOVER || thief.thiefState == Thief.ThiefState.ARRESTED || thief.thiefState == Thief.ThiefState.KILLED || thief.thiefState == Thief.ThiefState.STUN)
            return;
        if (Time.time > nextActionTime && thief.thiefState != Thief.ThiefState.STOP)
        {
            nextActionTime += period;
            // execute block of code here
            RefindPath();
        }

    }

    private void FixedUpdate()
    {
        if (thief.thiefState == Thief.ThiefState.HEAD_EXIT || thief.thiefState == Thief.ThiefState.HEAD_TREASURE || thief.thiefState == Thief.ThiefState.ESCAPE)
        {
            if (path == null) return;
            if (path.Length != 0)
            {
                currentWayPoint = path[0];
                //Debug.Log(path[0]);
                //Debug.Log(path.Length);
            }
            else currentWayPoint = transform.position;


            if (transform.position == currentWayPoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    thief.reachEscapePoint();                    //終点チェック
                }
                else
                {
                    currentWayPoint = path[targetIndex];
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, currSpeed * Time.deltaTime);
            //GetComponent<Rigidbody>().velocity = new Vector3(currentWayPoint.x - transform.position.x, 0, currentWayPoint.z - transform.position.z).normalized * currSpeed;
            //Debug.Log(GetComponent<Rigidbody>().velocity);
            transform.LookAt(new Vector3(currentWayPoint.x, transform.position.y, currentWayPoint.z));
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currentWayPoint, Vector3.up), 0.5f);
        }
    }
    //public void FollowPriority()
    //{
    //    StartCoroutine(FindAndFollow());
    //}

    public void tempIncreaseSpeed()
    {
        StartCoroutine(IncreaseSpeed(increasedSpeedDur));
    }

    public void GotStunned(float time)
    {
        StartCoroutine(Stunned(time));
    }

    public void HeadToEscapePoint()
    {
        SortEscapePoints();
        //int rand = Random.Range(0, escapePoints.Capacity);
        if(escapePointsByDistance.Count() > 1)
        {
            currTarget = escapePointsByDistance.ElementAt(0);
            PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound2);
        }
    }
    /// <summary>
    /// 一番近いのpointをgetする
    /// </summary>
    /// <returns></returns>
    //private Transform GetClosestEscapePoint()
    //{
    //    float closestdistance = 99999f;
    //    int closestCount = 0;
    //    for (int i = 0; i < escapePoints.Count; i++)
    //    {
    //        float distance = GameManager.Instance.getXZDistance(this.transform, escapePoints[i]);
    //        if (distance <= closestdistance)
    //        {
    //            closestdistance = distance;
    //            closestCount = i;
    //        }
    //    }

    //    return escapePoints[closestCount];
    //}
    void SortEscapePoints()
    {
        if (escapePoints != null)
            escapePointsByDistance = escapePoints.OrderBy(t => GameManager.Instance.getXZDistance(this.transform, t));  
    }


    public void HeadToTreasure()
    {
        currTarget = treasure;
        PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound);
    }

    public void HeadToExit()
    {
        currTarget = exit;
        PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            //mIsAllowFollow = true;
            path = newPath;
            //targetIndex = 0;
            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");

            //StopCoroutine(FollowPathCorotine);
            //FollowPathCorotine = null;
            //FollowPathCorotine = FollowPath();
            //StartCoroutine(FollowPathCorotine);
            
        }
        else
        {
            Debug.Log("what?");
            //StopCoroutine("FollowPath");
            //mIsAllowFollow = false;
            PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound2);
        }
    }

    public void OnPathFound2(Vector3[] newPath, bool pathSuccessful)//  for priority
    {
        //Debug.Log("hue");
        if (pathSuccessful)
        {
            //Debug.Log("pathSuccess");
            //if(thief.ghostCollider!=null)thief.ghostCollider.SetActive(true);
            path = newPath;
            targetIndex = 0;
            thief.setGhostCollider();
            //StopCoroutine("FollowPath");
            //StartCoroutine("FollowPath");

            //StopCoroutine(FollowPathCorotine);
            //FollowPathCorotine = null;
            //FollowPathCorotine = FollowPath();
            //StartCoroutine(FollowPathCorotine);

            pathFindIndex = 0;
            SortEscapePoints();
        }
        else
        {
            Debug.Log("inside else0");
            //全てのEscapePointを検査して、もし逃げる場所がない場合、鬼のBlockを解除する
            
            if (pathFindIndex < escapePointsByDistance.Count()-1)
            {

                
                Debug.Log("index " + pathFindIndex);
                currTarget = escapePointsByDistance.ElementAt(pathFindIndex);
                PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound2);
                pathFindIndex++;
                //
                Debug.Log("index after plus " + pathFindIndex +" "+"total escapePoints: " + escapePoints.Count);
            }
            else if (pathFindIndex >= escapePoints.Count-1)
            {

                if (thief.ghostCollider != null) thief.ghostCollider.radius = 1f; //鬼のBlockを解除する
                //thief.playerCollider.SetActive(false);

                //今の目標を鬼にする
                //currTarget = ghost;

                //今の目標を出口或いは宝に設置する
                if (thief.mIsTakenTreasure)
                {
                    currTarget = exit;
                }
                else
                {
                    currTarget = treasure;
                }
                pathFindIndex = 0;
                PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound);
                Debug.Log("!");
                //if (transform.position.x == currTarget.position.x && transform.position.z == currTarget.position.z)
                //{
                //    PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound);
                //}

            }
           
            //else if (pathFindIndex == escapePoints.Count)
            //{
            //    Debug.Log("hi");
            //    currTarget = ghost;
            //    PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound2);
            //}
            //int rand = Random.Range(0, escapePoints.Capacity);
            //while(currTarget == escapePoints[rand])
            //{
            //    rand = Random.Range(0, escapePoints.Capacity);
            //}
            //currTarget = escapePoints[rand];
            //PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound2);
        }
    }

    public void RefindPath()
    {
        PathRequestManager.RequestPath(transform.position, currTarget.position, OnPathFound);
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

   

    //IEnumerator FindAndFollow()
    //{
    //    //float temp = 0;
    //    int currIndex = 0;
    //    while (currIndex < prirityPoints.Count)
    //    {
    //        Debug.Log("called");
    //        PathRequestManager.RequestPath(transform.position, prirityPoints[currIndex].position, OnPathFound2);
    //        Debug.Log(mIsHeadable);
    //        //currSpeed = temp;
    //        //currSpeed = 0;
    //        //yield return new WaitForSeconds(0.2f);
    //        //currSpeed = temp;
    //        if (mIsHeadable)
    //        {
    //            Debug.Log("wee");
    //            PathRequestManager.RequestPath(transform.position, prirityPoints[currIndex].position, OnPathFound);
    //            yield break;
    //        }
    //        currIndex++;
    //    }

    //    if (!mIsHeadable)//All prioity targets not headable
    //    {
    //        //Do someting;
    //    }
    //}

   

    //IEnumerator FollowPath()
    //{
    //    Vector3 currentWayPoint;
    //    if (path.Length != 0)
    //    {
    //        currentWayPoint = path[0];
    //        //Debug.Log(path[0]);
    //        //Debug.Log(path.Length);
    //    }
    //    else currentWayPoint = transform.position;

    //    while (true)
    //    {
    //        if (transform.position == currentWayPoint)
    //        {
    //            targetIndex++;
    //            if (targetIndex >= path.Length)
    //            {
    //                thief.reachEscapePoint();                    //終点チェック
    //                yield break;
    //            }
    //            currentWayPoint = path[targetIndex];
    //        }
    //        transform.position = Vector3.MoveTowards(transform.position, currentWayPoint, currSpeed * Time.deltaTime);
    //        //GetComponent<Rigidbody>().velocity = new Vector3(currentWayPoint.x - transform.position.x, 0, currentWayPoint.z - transform.position.z).normalized * currSpeed;
    //        //Debug.Log(GetComponent<Rigidbody>().velocity);
    //        transform.LookAt(new Vector3(currentWayPoint.x, transform.position.y, currentWayPoint.z));
    //        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(currentWayPoint, Vector3.up), 0.5f);
    //        yield return new WaitForEndOfFrame();
    //    }
    //}


    IEnumerator Stunned(float sec)
    {
        Thief.ThiefState tempState = thief.thiefState;
        thief.thiefState = Thief.ThiefState.STUN;
        float temp = currSpeed;
        currSpeed = 0.0f;
        yield return new WaitForSeconds(sec);
        currSpeed = temp;
        thief.thiefState = tempState;
    }

    IEnumerator IncreaseSpeed(float sec)
    {
        float temp = currSpeed;
        currSpeed = increasedSpeed;
        yield return new WaitForSeconds(sec);
        currSpeed = initialSpeed;
    }
}
