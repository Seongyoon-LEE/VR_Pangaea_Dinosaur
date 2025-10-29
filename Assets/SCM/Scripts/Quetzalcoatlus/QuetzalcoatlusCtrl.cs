using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuetzalcoatlusCtrl : MonoBehaviour, IDinoCtrl
{
    private readonly int hashLeftRight = Animator.StringToHash("LeftRight");
    private readonly int hashUpDown = Animator.StringToHash("UpDown");
    private enum Status
    {
        PATROL, ATTACK
    }

    [SerializeField] private Status status = Status.PATROL;

    PatrolPoints path;
    int idx = 0;
    float moveSpeed = 5f;
    float rotSpeed = 10f;
    private Transform playerTr;
    Animator animator;
    WaitForSeconds ws;

    float dampTime = 3f;
    [SerializeField] float curTime = 0f;
    [SerializeField] float updownValue;
    [SerializeField] float leftrightValue;
    void Start()
    {
        path = GameObject.Find("QuetzalcoatlusPoints").GetComponent<PatrolPoints>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        ws = new WaitForSeconds(0.1f);
        idx = Random.Range(0, path.GetWayCount());
        transform.position = path.GetWayPoint(idx, transform);

        //StartCoroutine(UpdateCurrentStatus());
    }

    public IEnumerator UpdateCurrentStatus()
    {
        while (true)
        {
            yield return ws;

            switch (status)
            {
                case Status.PATROL:
                    OnPatrol();
                    break;
                case Status.ATTACK:
                    OnAttack();
                    break;
            }
        }
    }

    private void Update()
    {
        switch (status)
        {
            case Status.PATROL:
                OnPatrol();
                break;
            case Status.ATTACK:
                OnAttack();
                break;
        }
    }

    private void LateUpdate()
    {
        if (status == Status.ATTACK)
        {
            animator.SetFloat(hashLeftRight, 0.5f);
            animator.SetFloat(hashUpDown, 0.5f);
        }

        if (status == Status.PATROL)
        {
            curTime += Time.deltaTime;

            if (curTime > dampTime)
            {
                updownValue = Random.value;
                leftrightValue = Random.value;
                curTime = 0f;
            }
            animator.SetFloat(hashLeftRight, leftrightValue, dampTime, Time.deltaTime);
            animator.SetFloat(hashUpDown, updownValue, dampTime, Time.deltaTime);
        }
        
    }
    public void OnPatrol()
    {
        Vector3 movePos = path.GetWayPoint(idx) - transform.position;
        movePos.y = 0f;

        if (movePos != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movePos), Time.fixedDeltaTime * rotSpeed);
        }

        transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
        if (Vector3.Distance(path.FlattenY(path.GetWayPoint(idx)), path.FlattenY(transform.position)) < 0.5f)
        {
            idx = path.CurrentWayPoint(idx);
        }
    }

    public void OnAttack()
    {
        if (playerTr == null) return;
        Vector3 movePos = playerTr.position - transform.position;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movePos), Time.fixedDeltaTime * rotSpeed);
        transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
    }

    public void FindOut(Transform tr)
    {
        status = Status.ATTACK;
        playerTr = tr;
        moveSpeed = 30f;
    }
    public void OnTrace()
    {
        throw new System.NotImplementedException();
    }

    public void OnIdle()
    {
        throw new System.NotImplementedException();
    }

    
}
