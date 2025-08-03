using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : MonoBehaviour
{
    [Header("Property Knight")]
    public int dame;
    public int armor;
    public float moveSpeed;
    public float maxHeal;
    [SerializeField]
    public float currentHeal { get; set; }
    public int timerespawn;
    private Transform slider;
    [HideInInspector]
    public GameObject target = null; 
    [HideInInspector]
    public Animator _anim; 
    [HideInInspector]
    public bool allowMove = true;
    public Vector3 positionFlag { get; set; }
    private Barrack _barrack;
    public List<GameObject> ListEnemy = new List<GameObject>();
    [Header("Tầm phát hiện mục tiêu của Knight")]
    public float radius;
    private bool isDead = false;

    private bool isAttacking = false;
    private float timeregend = 0; 

    public EnemyController _target = null;
    public bool MustReturn = false;
    private EnemyController _enemycode;
    private SpriteRenderer _sprite;
    void Awake()
    {
        slider = transform.Find("HealBar").transform.Find("slider");
        _anim = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.2f);
        currentHeal = maxHeal;
        _barrack = transform.parent.gameObject.GetComponent<Barrack>();
        UpgradeProperty();
    }
    private void UpgradeProperty()
    {
        if (PlayerPrefs.GetInt("Barrackupgrade1") == 1)
        {
            maxHeal = maxHeal + (maxHeal / 10);
            currentHeal = maxHeal;
        }
        if (PlayerPrefs.GetInt("Barrackupgrade2") == 1)
        {
            armor = armor + 1;
        }
        if (PlayerPrefs.GetInt("Barrackupgrade3") == 1)
        {
            timerespawn = timerespawn - 2;
        }

        if (PlayerPrefs.GetInt("Barrackupgrade4") == 1)
        {
            maxHeal = maxHeal + 0.2f * maxHeal;
            currentHeal = maxHeal;
        }
        if (PlayerPrefs.GetInt("Barrackupgrade5") == 1)
        {
            dame += 2;
        }

    }
    void UpdateTarget()
    {
        if (isDead) return;
        if (target == null || allowMove == true) MustReturn = false;
        if (allowMove == false) return;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyController>() != null)
            {
                _enemycode = enemy.GetComponent<EnemyController>();
            }

            if (enemy == null) return;
            float range = Vector2.Distance(transform.position, enemy.transform.position);
            if (range <= radius && _enemycode.currentHeal > 0)
            {
                if (!ListEnemy.Contains(enemy) && enemy != null)
                {
                    ListEnemy.Add(enemy);
                }
            }
            else 
                ListEnemy.Remove(enemy);
        }
        RemoveNull();
       
        foreach (GameObject enemy in ListEnemy)
        {
            if (enemy == null) return;
            if (enemy.GetComponent<EnemyController>()._KnightToFight == gameObject)
            {
                SetTarget(enemy);
                MustReturn = true;
                return;
            }
        }
        if (MustReturn == true) return;
        if (ListEnemy.Count == 1) 
        {
            isAttacking = false;
            if (ListEnemy[0] == null) return;
            SetTarget(ListEnemy[0]);
            if (CheckEnemyToFight4Ever(this, ListEnemy[0]))
            {
                MustReturn = true;
            }
            else if (!CheckEnemyToFight4Ever(this, ListEnemy[0]) && ListEnemy[0].GetComponent<EnemyController>()._KnightToFight == null)
            {
                isAttacking = false;
                ListEnemy[0].GetComponent<EnemyController>()._KnightToFight = this.gameObject;
            }
        }
        if (MustReturn == true) return;
        else if (ListEnemy.Count == 2)
        {
            if (ListEnemy[0] == null || ListEnemy[1] == null) return;
            isAttacking = false;
            RemoveNull();
            for (int i = 0; i <= 1; i++)
            {

                if (CheckEnemyToFight4Ever(this, ListEnemy[i]) == true) 
                {
                    print("da chon duoc Enemy o vong for thu" + i);
                    SetTarget(ListEnemy[i]);
                    MustReturn = true;
                    break;
                }
                else if (!CheckEnemyToFight4Ever(this, ListEnemy[i]) && ListEnemy[i].GetComponent<EnemyController>()._KnightToFight == null)
                {
                    if (MustReturn == true) return;
                    ListEnemy[i].GetComponent<EnemyController>()._KnightToFight = this.gameObject;
                    SetTarget(ListEnemy[i]);
                    MustReturn = true;
                    break;
                }

            }
            if (!CheckEnemyToFight4Ever(this, ListEnemy[0]) && !CheckEnemyToFight4Ever(this, ListEnemy[1]) && target == null)
            {
                SetTarget(ListEnemy[1]);
                MustReturn = true;
            }
        }
        else if (ListEnemy.Count >= 3)
        {
            RemoveNull();
            isAttacking = false;
            if (ListEnemy[0] == null || ListEnemy[1] == null || ListEnemy[2] == null) return;

            foreach (GameObject enemy in ListEnemy)
            {

                if (CheckEnemyToFight4Ever(this, enemy)) 
                {
                    SetTarget(enemy);
                    MustReturn = true;
                    break;
                }
            }
            if (MustReturn == true) return;
            foreach (GameObject enemy in ListEnemy)
            {
                // RemoveNull();
                if (!CheckEnemyToFight4Ever(this, enemy) && enemy.GetComponent<EnemyController>()._KnightToFight == null) 
                {
                    if (MustReturn == true) return;
                    enemy.GetComponent<EnemyController>()._KnightToFight = this.gameObject;
                    SetTarget(enemy);
                    MustReturn = true;
                    break;
                }
            }
            if (!CheckEnemyToFight4Ever(this, ListEnemy[0]) && !CheckEnemyToFight4Ever(this, ListEnemy[1]) && !CheckEnemyToFight4Ever(this, ListEnemy[2]) && target == null)
            {
                SetTarget(ListEnemy[Random.Range(0, 3)]);
                MustReturn = true;
            }
        }
    }
    private void RegendHeal()
    {
        timeregend += Time.deltaTime;
        if (timeregend >= 3)
        {
            if (currentHeal > 0 && currentHeal < maxHeal)
            {
                currentHeal += Time.deltaTime * 4;
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, 1, 1);
            }
            else if (currentHeal >= maxHeal)
            {
                currentHeal = maxHeal;
                slider.transform.localScale = new Vector3(1, 1, 1);
                timeregend = 0f;
            }
        }
    }
    void Update()
    {
        if (Manager.isFinishing == true) return;
        int layer = Mathf.Clamp(Mathf.Abs(50 - Mathf.RoundToInt((transform.position.y - 1) * 20)), 1, 150);
        _sprite.sortingOrder = layer;
        if (isDead) return;
        if (target == null)
        {
            if (gameObject.transform.position == positionFlag) 
            {
                if (!isAttacking)
                {
                    _anim.SetInteger("Change", 0);
                }
                allowMove = true;
                RegendHeal();
            }
        }
        else 
        {
            if (gameObject.transform.position == positionFlag) 
            {
                allowMove = true;
            }
            if (Vector2.Distance(transform.position, target.transform.position) > 1f) 
            {
                target = null;
                return;
            }
            CheckFlipWith(target);
            if (Vector2.Distance(transform.position, target.transform.position) > 1f) return;
            if (transform.localScale.x == 1)
            {
                if (transform.position != target.transform.position + new Vector3(-0.2f, 0, 0) && allowMove)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position + new Vector3(-0.2f, 0, 0), Time.deltaTime * moveSpeed);
                    _anim.SetInteger("Change", 1);
                    if (Mathf.Abs(transform.position.x - target.transform.position.x) <= 0.3f && Mathf.Abs(transform.position.y - target.transform.position.y) <= 0.2f)
                    {
                        AttackEnemy();
                    }
                }
                else
                {
                    AttackEnemy();
                }
            }
            else if (transform.localScale.x == -1)
            {
                if (transform.position != target.transform.position + new Vector3(0.2f, 0, 0) && allowMove)
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position + new Vector3(0.2f, 0, 0), Time.deltaTime * moveSpeed);
                    _anim.SetInteger("Change", 1);
                    if (Mathf.Abs(transform.position.x - target.transform.position.x) <= 0.3f && Mathf.Abs(transform.position.y - target.transform.position.y) <= 0.2f)
                    {
                        AttackEnemy();
                    }
                }
                else
                {
                    AttackEnemy();
                }
            }
        }
    }
    public void CheckFlipWith(GameObject enemy)
    {
        if (transform.position.x <= enemy.transform.position.x)
        {
            if (transform.localScale.x == -1)
            {
                transform.localScale = new Vector3(1, 1, 1);
                slider.parent.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            }
        }
        else
        {
            if (transform.localScale.x == 1)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                slider.parent.transform.localScale = new Vector3(-0.3f, 0.3f, 1f);
            }
        }
    }
    private void SetTarget(GameObject enemy)
    {
        target = enemy;
        _target = target.GetComponent<EnemyController>();
    }
    private void AttackEnemy()
    {
        if (isAttacking == false)
        {
            isAttacking = true;
            SoundTower._instance.KnightFight();
  
            if (target.GetComponent<EnemyController>().StartAttack == false)
            {
                target.GetComponent<EnemyController>().StartAttack = true;
            }
            _anim.SetInteger("Change", 2);
            Invoke("setISAttackingfalse", 1f);
        }
    }
    private void setISAttackingfalse()
    {
        isAttacking = false;
        _anim.SetInteger("Change", 0); 
    }
    private bool CheckEnemyToFight4Ever(KnightController knight, GameObject enemy)
    {
        GameObject KnightToFight = enemy.GetComponent<EnemyController>()._KnightToFight;
        if (KnightToFight == knight.gameObject)
        {
            return true;
        }
        else
            return false;
    }
    public void GiveDame()
    {
        if (_target != null)
        {
            _target.TakeDamePhysic(dame);
            if (_target.currentHeal <= 0)
            {
                target = null;
                _target = null;
            }
        }
        else return;
    }
    public void TakeDame(int _dame)
    {
        if (currentHeal > 0)
        {
            int trueDame = _dame - armor;
            currentHeal -= trueDame;
            if (currentHeal > 0)
                slider.transform.localScale = new Vector3((1 / maxHeal) * currentHeal, transform.localScale.y, transform.localScale.z);
            else
                Die();
        }
        else
            Die();
    }
    private void Die()
    {
        isDead = true;
        _barrack.StartRespawn(timerespawn);
        _anim.SetInteger("Change", 3);
        Destroy(gameObject, 1f);
    }
    private void RemoveNull()
    {

        for (int i = 0; i < ListEnemy.Count; i++)
        {
            if (ListEnemy[i] == null)
                ListEnemy.Remove(ListEnemy[i]);
        }
        //foreach(GameObject enemy in ListEnemy)
        //{
        //    if (enemy == null)
        //    {
        //        ListEnemy.Remove(enemy);
        //    }
        //}
    }
}
