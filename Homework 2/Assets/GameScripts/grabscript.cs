using UnityEngine;

public class HandAnimationController : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            anim.SetBool("isGrabbing", true);
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            anim.SetBool("isGrabbing", false);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            anim.SetBool("isDante", true);
        }

        if (Input.GetKeyUp(KeyCode.V))
        {
            anim.SetBool("isDante", false);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            anim.SetBool("isTrigger", true);
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            anim.SetBool("isTrigger", false);
        }
    }
}