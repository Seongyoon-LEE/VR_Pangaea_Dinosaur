using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeinocheirusCtrl : MonoBehaviour, IDinoCtrl
{
    private readonly int hashMove = Animator.StringToHash("Move");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly string transparentLayer = "Transparent";
    private readonly string dinoLayer = "DINO";
    public enum Status
    {
        None, PATROL, TRACE, ATTACK, STUN
    }
    
    public Status status = Status.PATROL;

    Animator animator;
    NavMeshAgent agent;
    WaitForSeconds ws;
    private Transform playerTr;
    PatrolPoints path;
    int idx = 0;
    public float runSpeed = 5f;
    float rotSpeed = 10f;
    Light _light;
    public Renderer[] allRenderers;
    float fadeDuration = 3f;
    bool isFade = false;

    Coroutine fadeCoroutine = null;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ws = new WaitForSeconds(0.3f);
        path = GameObject.Find("DeinocheirusPoints").GetComponent<PatrolPoints>();
        allRenderers = GetComponentsInChildren<Renderer>();
        _light = GetComponentInChildren<Light>();
        StartCoroutine(UpdateCurrentStatus());
        SetAlpha(0);

        agent.enabled = false;
        transform.position = path.GetWayPoint(idx);
        agent.enabled = true;
    }

    public void FindOut(Transform tr)
    {
        playerTr = tr;
        FadeIn();
    }

    public void PlayerLeave()
    {
        status = Status.PATROL;
        playerTr = null;
        fadeCoroutine = null;
        FadeOut();
    }
    public IEnumerator UpdateCurrentStatus()
    {
        while (true)
        {
            yield return ws;

            if (isFade)
                status = Status.None;

            switch (status)
            {
                case Status.PATROL:
                    OnPatrol();
                    break;
                case Status.TRACE:
                    OnTrace();
                    break;
                case Status.ATTACK:
                    OnAttack();
                    break;
                default:
                    OnIdle();
                    break;
            }
        }
    }

    public void OnPatrol()
    {
        agent.isStopped = false;
        agent.destination = path.GetWayPoint(idx);
        animator.SetFloat(hashMove, 0.5f);
        if (Vector3.Distance(path.FlattenY(path.GetWayPoint(idx)), path.FlattenY(transform.position)) < 5f)
        {
            idx = path.CurrentWayPoint(idx);
        }
    }

    public void OnTrace()
    {
        if (Vector3.Distance(path.FlattenY(playerTr.position), path.FlattenY(transform.position)) < 4f)
        {
            status = Status.ATTACK;
            return;
        }

        animator.SetFloat(hashMove, 1f);
        animator.SetBool(hashAttack, false);
        agent.isStopped = false;
        agent.speed = runSpeed;
        agent.destination = playerTr.position;
    }

    public void OnAttack()
    {
        if (playerTr == null) return;

        animator.SetBool(hashAttack, true);
        agent.isStopped = true;

        Vector3 taget = (playerTr.position - transform.position).normalized;

        Quaternion rot = Quaternion.LookRotation(taget);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotSpeed);
    }

    public void OnIdle()
    {
        agent.isStopped = true;
    }

    private void SetLayerRecursive(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, newLayer);
        }
    }

    private void SetAlpha(float alpha)
    {
        //_light.intensity = alpha > 0 ? 1000*alpha : 0;

        foreach (Renderer renderer in allRenderers)
        {
            foreach (Material m in renderer.materials)
            {
                Color color = m.color;
                color.a = alpha;
                m.color = color;
            }
        }
    }

    void FadeIn()
    {
        if (fadeCoroutine != null) return;
        fadeCoroutine = StartCoroutine(FadeInCoroutine());
    }

    void FadeOut()
    {
        if (fadeCoroutine != null) return;
        fadeCoroutine = StartCoroutine(FadeOutCoroutine());
    }
    IEnumerator FadeInCoroutine()
    {
        isFade = true;
        animator.SetFloat(hashMove, 0f);
        SetLayerRecursive(gameObject, 0);

        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / fadeDuration;

            float newAlpha = Mathf.Lerp(0f, 1f, t);
            print(newAlpha);
            SetAlpha(newAlpha);

            yield return null;
        }

        SetAlpha(1f);
        isFade = false;
        status = Status.TRACE;
    }

    IEnumerator FadeOutCoroutine()
    {
        float timeElapsed = 0f;
        animator.SetFloat(hashMove, 0f);

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / fadeDuration;

            float newAlpha = Mathf.Lerp(1f, 0f, t);
            SetAlpha(newAlpha);

            yield return null;
        }

        SetAlpha(0f);

        SetLayerRecursive(gameObject, LayerMask.NameToLayer(transparentLayer));
    }
}
