using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabol : MonoBehaviour
{
    public bool isArrow;
    public GameObject partical;
    private GameObject _partical;
    public GameObject top_arrow; 
    public GameObject target;
    public int accuracy_mode = 1;
    public float maxLaunch = 4f; 
    private bool sw = false;
    private bool actived = false;
    public Vector3 latePos = new Vector3(0, 0, 0);
    [Header("Properties boom")]
    public bool isBoom;
    public GameObject boom;

    public int Dame { get; set; }
    private Bullet_Arrow _bulletArrow;

    public float lifeTime = 3f;
    public float speed = 0.5f;
    public float speedUpOverTime = 0.1f;
    public float ballisticOffset = 0.5f;
    public bool freezeRotation = false;

    private Vector2 originPoint;
    private Vector2 aimPoint;
    private Vector2 myVirtualPosition;
    private Vector2 myPreviousPosition;
    private float counter;
    private SpriteRenderer sprite;
    private Rigidbody2D _rigid;
    void Start()
    {
        _rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        if (GetComponent<Bullet_Arrow>() != null)
        {
            _bulletArrow = GetComponent<Bullet_Arrow>();
        }
        if (partical != null)
        {
            _partical = Instantiate(partical, transform.position, Quaternion.identity);
            if (_partical.GetComponent<Explosion>() != null)
            {
                _partical.GetComponent<Explosion>().dame = Dame;
            }
            _partical.SetActive(false);
        }
        sw = true;
        if (target == null)
        {
            Destroy(gameObject);
        }
        else
        {
            aimPoint = target.transform.position;
        }
        //_enemy = target.GetComponent<EnemyController>();

        originPoint = myVirtualPosition = myPreviousPosition = transform.position;

    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject == target)
        {
            TurnOnTopArrow();
            StopFalling();
            Invoke("onDestroy", 0.5f);
        }
    }
    private void StopFalling()
    {
        sw = false;
        InstancePartical();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
    }
    private void InstancePartical()
    {
        if (_partical != null)
        {
            if (isArrow && target != null)
            {
                if (target.GetComponent<EnemyController>() != null)
                {
                    if (target.GetComponent<EnemyController>().currentHeal > 0)
                    {
                        _partical.transform.position = target.transform.position;
                        _partical.SetActive(true);
                    }
                }
                else
                {
                    Destroy(_partical);
                }
            }
            else if (isArrow && target == null)
            {
                Destroy(_partical);
            }

            if (!isArrow)
            {
                _partical.transform.position = transform.position;
                _partical.SetActive(true);
            }

        }
    }
    void Update()
    {
        if (target != null)
        {
            latePos = target.transform.position;
            if (actived == false) 
            {
                actived = true;
                PreLaunch();
            }
            else
            {
                if (sw == true)
                {
                    if (_rigid.isKinematic == false)
                    {
                        Move();
                    }                
                }
            }
        }
        else
        {
            find_erro();
        }
    }
    void onDestroy()
    {
        Destroy(gameObject);
    }
    private void TurnOnTopArrow()
    {
        if (top_arrow != null)
        {
            top_arrow.SetActive(true);
        }   
    }
    private void PreLaunch()
    {
        float xTarget = target.transform.position.x;
        float yTarget = target.transform.position.y;
        float xCurrent = transform.position.x;
        float yCurrent = transform.position.y;
        float xDistance = Mathf.Abs(xTarget - xCurrent);
        float yDistance = yTarget - yCurrent;
        float fireAngle = 1.57075f - (float)(Mathf.Atan((Mathf.Pow(maxLaunch, 2f) + Mathf.Sqrt(Mathf.Pow(maxLaunch, 4f) - 9.8f * (9.8f * Mathf.Pow(xDistance, 2f) + 2f * yDistance * Mathf.Pow(maxLaunch, 2f)))) / (9.8f * xDistance)));
        float xSpeed = (float)Mathf.Sin(fireAngle) * maxLaunch;
        float ySpeed = (float)Mathf.Cos(fireAngle) * maxLaunch;
        if ((xTarget - xCurrent) < 0f) { xSpeed = -xSpeed; }            
        Calculation(ySpeed);                                           
        sw = true;
    }
    private void Calculation(float speedy)
    {
        next_position(Time.time % ((speedy / 9.81f) * 2));
    }
    private void next_position(float airtime)
    {
        float xTarget = target.transform.position.x;
        float yTarget = target.transform.position.y;
        float speedy = target.GetComponent<Rigidbody2D>().velocity.y;
        float speedx = target.GetComponent<Rigidbody2D>().velocity.x;
        Launch(xTarget + (speedx * airtime), yTarget + (speedy * airtime));
    }
    private void Launch(float xTarget, float yTarget)
    {
        GetComponent<Rigidbody2D>().isKinematic = false;
        float xCurrent = transform.position.x;
        float yCurrent = transform.position.y;
        float xDistance = Mathf.Abs(xTarget - xCurrent);
        float yDistance = yTarget - yCurrent;
        float fireAngle = 1.57075f - (float)(Mathf.Atan((Mathf.Pow(maxLaunch, 2f) + Mathf.Sqrt(Mathf.Pow(maxLaunch, 4f) - 9.8f * (9.8f * Mathf.Pow(xDistance, 2f) + 2f * yDistance * Mathf.Pow(maxLaunch, 2f)))) / (9.8f * xDistance)));
        float xSpeed = (float)Mathf.Sin(fireAngle) * maxLaunch;
        float ySpeed = (float)Mathf.Cos(fireAngle) * maxLaunch;
        if ((xTarget - xCurrent) < 0f) { xSpeed = -xSpeed; }          
        if (!float.IsNaN(xSpeed) && !float.IsNaN(ySpeed))
        {
            GetComponent<Rigidbody2D>().velocity = new Vector3(xSpeed, ySpeed, 0f);
        }
        else
        {
            maxLaunch = maxLaunch + 0.3f;
            PreLaunch();
        }
    }
    private void simulateRotation()
    {
        Vector3 velocity = GetComponent<Rigidbody2D>().velocity;
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion tempRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion newRotation = this.gameObject.transform.rotation;
        newRotation.x = Mathf.LerpAngle(newRotation.x, tempRotation.x, 1000000);
        newRotation.y = Mathf.LerpAngle(newRotation.y, tempRotation.y, 1000000);
        newRotation.z = Mathf.LerpAngle(newRotation.z, tempRotation.z, 1000000);
        newRotation.w = Mathf.LerpAngle(newRotation.w, tempRotation.w, 1000000);
        this.gameObject.transform.rotation = newRotation;
    }
    private void find_erro()
    {
        if (target != null)
        {
            if (this.transform.position.y < target.transform.position.y)
            {
                sw = false;
                Vector3 rotation = this.gameObject.transform.eulerAngles;
                GetComponent<Rigidbody2D>().isKinematic = true;
                GetComponent<Collider2D>().enabled = false;
                this.gameObject.transform.localEulerAngles = rotation;
                Invoke("onDestroy", 1f);
            }
        }
        else
        {
            if (sw == true)
            {
                if (GetComponent<Rigidbody2D>().bodyType != RigidbodyType2D.Static)
                {
                     simulateRotation();
                }
                transform.position = Vector2.MoveTowards(transform.position, latePos, Time.deltaTime * accuracy_mode);
                if (GetComponent<Rigidbody2D>().velocity.y < 0)
                {
                    isFallingNoTarget();
                }
            }

        }
    }
    private void isFalling()
    {
        if (this.transform.position.y < target.transform.position.y) 
        {
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            if (isBoom)
            {
                Boom_Explosion();
            }
        }
        float auxv = 0f; 
        float speedy = target.GetComponent<Rigidbody2D>().velocity.y;
        float speedx = target.GetComponent<Rigidbody2D>().velocity.x;
        if (Mathf.Sqrt(speedy * speedy) > Mathf.Sqrt(speedx * speedx))
        {
            auxv = speedy;
        }
        else
        {
            auxv = speedx;
        }

        if (auxv < 0) { auxv = -auxv; } 
        find_erro();
    }
    private void isFallingNoTarget()
    {
        if (this.transform.position.y < latePos.y)
        {
            if (!isArrow)
            {
                InstancePartical();
                Invoke("onDestroy", 0.3f);
            }
            Invoke("onDestroy", 0.5f); 
            int _random = Random.Range(-100, -80);
            transform.rotation = Quaternion.Euler(0,0,_random);
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            GetComponent<Collider2D>().enabled = false;
        }
    }
    private void Boom_Explosion()
    {
        InstancePartical();
        GameObject _boom = Instantiate(boom, transform.position, Quaternion.identity);
        _boom.GetComponent<Boom_Explosion>().dame = Dame;
        onDestroy();
    }
    private void Move()
    {
        counter += Time.fixedDeltaTime;
        speed += Time.fixedDeltaTime * speedUpOverTime;
        if (target != null)
        {
            aimPoint = target.transform.position;
        }
        Vector2 originDistance = aimPoint - originPoint;
        Vector2 distanceToAim = aimPoint - (Vector2)myVirtualPosition; 
        myVirtualPosition = Vector2.Lerp(originPoint, aimPoint, counter * speed / originDistance.magnitude);
        transform.position = AddBallisticOffset(originDistance.magnitude, distanceToAim.magnitude);
        LookAtDirection2D((Vector2)transform.position - myPreviousPosition);
        myPreviousPosition = transform.position;
        sprite.enabled = true;
    }
    private Vector2 AddBallisticOffset(float originDistance, float distanceToAim)
    {
        if (ballisticOffset > 0f)
        {
            float offset = Mathf.Sin(Mathf.PI * ((originDistance - distanceToAim) / originDistance));
            offset *= originDistance;
            return (Vector2)myVirtualPosition + (ballisticOffset * offset * Vector2.up);
        }
        else
        {
            return myVirtualPosition;
        }
    }
    private void LookAtDirection2D(Vector2 direction)
    {
        if (freezeRotation == false)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

}
