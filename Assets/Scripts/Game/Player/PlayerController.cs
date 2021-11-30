using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] float constantSpeedForce;
    [SerializeField] float forwardBoostForce;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerAbilities pAbilities;
    [SerializeField] GameObject playerObject;
    [SerializeField] bool frozen;
    [SyncVar(hook = nameof(PlayerColorChanged))] public Color playerColor;
    
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
    }

    public void PlayerColorChanged(Color oldColor, Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
    }

    private void FixedUpdate()
    {
        if (!hasAuthority || frozen) return;

        rb.AddForce(transform.up * constantSpeedForce * Time.deltaTime);

        if (Input.GetKey(KeyCode.W))
        {
            Boost(transform.up);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Boost(-transform.right);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Boost(transform.right);
        }
    }

    [ClientRpc]
    public void RpcFreezePlayer()
    {
        frozen = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    }

    [ClientRpc]
    public void RpcUnFreezePlayer()
    {
        Debug.Log("RpcUnfreezing...");
        frozen = false;
        rb.constraints = RigidbodyConstraints2D.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority || frozen) return;

        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }

    void Boost(Vector3 direction)
    {
        rb.AddForce(direction * forwardBoostForce * Time.deltaTime);
    }
}
