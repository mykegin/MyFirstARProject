using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class MonsterProximityAttackAR : MonoBehaviour
{
    public float enterAttackRange = 1.2f;
    public float exitAttackRange = 1.6f;
    public string animatorBoolName = "isAttacking";
    public bool requireTracked = true;
    public bool allowFindInChildren = true;

    private Animator anim;
    private static List<MonsterProximityAttackAR> all = new List<MonsterProximityAttackAR>();
    private bool attacking = false;
    private ObserverBehaviour obs;
    private Coroutine attackRoutine;
    private bool waiting = false;
    private int attackCount = 0;
    private bool scaled = false;
    private static bool someoneScaled = false;

    void OnEnable() // Adding the monster to the list once he appears on the screen
    {
        if (!all.Contains(this)) all.Add(this);
    }

    void OnDisable() // Removing the monster from the list once he disappears from the screen
    {
        all.Remove(this);
    }

    void Awake() // To get the animator component
    {
        anim = GetComponent<Animator>();
        if (anim == null && allowFindInChildren)
            anim = GetComponentInChildren<Animator>();

        obs = GetComponentInParent<ObserverBehaviour>();

        if (exitAttackRange <= enterAttackRange)
            exitAttackRange = enterAttackRange + 0.4f;
    }

    void Update()

        /*
        Here are made to following checks: 
         - if there is only one monster it looks at the screen
         - if there are multiple monsters they look at eachother
         - if they are close enough they start fighting
        */
    {
        if (anim == null) return;
        if (requireTracked && !IsTracked())
        {
            SetAttack(false);
            LookAtCam();
            return;
        }

        var other = GetNearest();
        if (other == null)
        {
            SetAttack(false);
            LookAtCam();
            return;
        }

        float dist = Vector3.Distance(transform.position, other.transform.position);
        bool shouldAttack = attacking ? dist <= exitAttackRange : dist <= enterAttackRange;

        HandleAttack(shouldAttack);
        LookAtTarget(other);
    }

    void HandleAttack(bool should) // Setting random delays for attacks
    {
        if (should)
        {
            if (!attacking && !waiting)
            {
                float delay = Random.Range(0.3f, 2f);
                attackRoutine = StartCoroutine(AttackCycle(delay));
            }
        }
        else
        {
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
                waiting = false;
            }
            SetAttack(false);
        }
    }

    IEnumerator AttackCycle(float delay) // Attack counter
    {
        waiting = true;
        yield return new WaitForSeconds(delay);
        SetAttack(true);
        yield return new WaitForSeconds(1.5f);
        SetAttack(false);
        waiting = false;
        attackRoutine = null;

        attackCount++;

        if (attackCount >= 5 && !scaled && !someoneScaled)
        {
            transform.localScale *= 3.5f;
            scaled = true;
            someoneScaled = true;
        }

        var n = GetNearest();
        if (n != null)
        {
            float d = Vector3.Distance(transform.position, n.transform.position);
            if (d <= exitAttackRange)
            {
                float next = Random.Range(0.3f, 2f);
                attackRoutine = StartCoroutine(AttackCycle(next));
            }
        }
    }

    MonsterProximityAttackAR GetNearest() // Searching for the nearest monster
    {
        MonsterProximityAttackAR near = null;
        float min = float.MaxValue;
        foreach (var m in all)
        {
            if (m == this) continue;
            if (m == null) continue;
            if (!m.gameObject.activeInHierarchy) continue;
            if (requireTracked && !m.IsTracked()) continue;

            float d = Vector3.Distance(transform.position, m.transform.position);
            if (d < min)
            {
                min = d;
                near = m;
            }
        }
        return near;
    }

    bool IsTracked()
    {
        if (!requireTracked) return true;
        if (obs == null) return true;
        string s = obs.TargetStatus.Status.ToString().ToUpperInvariant();
        return s.Contains("TRACK") || s.Contains("DETECT") || s.Contains("EXTEND");
    }

    void SetAttack(bool v)
    {
        if (attacking == v) return;
        attacking = v;

        bool found = false;
        foreach (var p in anim.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Bool && p.name == animatorBoolName)
            {
                found = true;
                break;
            }
        }

        if (!found) return;
        anim.SetBool(animatorBoolName, v);
    }

    void LookAtTarget(MonsterProximityAttackAR o)
    {
        if (o == null || (requireTracked && !o.IsTracked()))
        {
            LookAtCam();
            return;
        }

        Vector3 dir = (o.transform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion r = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, r, Time.deltaTime * 3f);
        }
    }

    void LookAtCam()
    {
        if (Camera.main == null) return;
        Vector3 dir = (Camera.main.transform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion r = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, r, Time.deltaTime * 2f);
        }
    }

    void OnDrawGizmosSelected() // Attacking range visualization
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enterAttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, exitAttackRange);
    }
}
