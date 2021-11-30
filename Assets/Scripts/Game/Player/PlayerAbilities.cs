using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : NetworkBehaviour
{
    [SerializeField] Rigidbody2D rb;
    public void Update()
    {
        if (!hasAuthority) return;
    }

    public IEnumerator freezePlayer(float duration)
    {
        rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(duration);
        rb.constraints = RigidbodyConstraints2D.None;
    }
}
