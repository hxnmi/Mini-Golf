using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float force;
    [SerializeField] LineRenderer aimLine;
    [SerializeField] Transform aimWorld;
    bool shoot;
    bool shootingMode = true;
    float forceFactor;
    Vector3 forceDirection;
    Ray ray;
    Plane plane;

    int shootCount;
    public UnityEvent<int> onBallShooted = new UnityEvent<int>();
    public UnityEvent OnBallHitWall;

    public bool ShootingMode { get => shootingMode; }
    public int ShootCount { get => shootCount; }

    // Update is called once per frame
    void Update()
    {
        if (shootingMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                aimLine.gameObject.SetActive(true);
                aimWorld.gameObject.SetActive(true);
                plane = new Plane(Vector3.up, this.transform.position);
            }
            else if (Input.GetMouseButton(0))
            {
                // force direction
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                plane.Raycast(ray, out var distance);
                forceDirection = this.transform.position - ray.GetPoint(distance);
                forceDirection.Normalize();

                // force factor
                var mouseViewportPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                var ballViewportPos = Camera.main.WorldToViewportPoint(this.transform.position);
                var pointerDirection = ballViewportPos - mouseViewportPos;
                pointerDirection.z = 0;
                pointerDirection.z *= Camera.main.aspect;
                pointerDirection.z = Mathf.Clamp(pointerDirection.z, -0.5f, 0.5f);
                forceFactor = pointerDirection.magnitude * 2;

                //aim visual
                aimWorld.transform.position = this.transform.position;
                aimWorld.forward = forceDirection;
                aimWorld.localScale = new Vector3(1, 1, 0.5f + forceFactor);

                var ballScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);
                var mouseScreenPos = Input.mousePosition;
                ballScreenPos.z = 1;
                mouseScreenPos.z = 1;
                var positions = new Vector3[]
                {
                                        Camera.main.ScreenToWorldPoint(ballScreenPos),
                                        Camera.main.ScreenToWorldPoint(mouseScreenPos)
                };
                aimLine.SetPositions(positions);
                aimLine.endColor = Color.Lerp(Color.green, Color.red, forceFactor);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                shoot = true;
                shootingMode = false;
                aimLine.gameObject.SetActive(false);
                aimWorld.gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (shoot)
        {
            shoot = false;
            shootCount += 1;
            onBallShooted.Invoke(shootCount);
            rb.AddForce(forceDirection * force * forceFactor, ForceMode.Impulse);
        }
        if (rb.velocity.sqrMagnitude < 2f && rb.velocity.sqrMagnitude > 0)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public bool IsMove()
    {
        return rb.velocity != Vector3.zero;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (this.IsMove())
            return;
        shootingMode = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            OnBallHitWall.Invoke();
        }
    }
}