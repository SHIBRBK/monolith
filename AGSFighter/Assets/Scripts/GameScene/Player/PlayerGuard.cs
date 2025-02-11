using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGuard : MonoBehaviour
{
    [SerializeField]
    private ColAnimationEvent enemyColAnim;
    [SerializeField]
    private ColAnimationEvent colAnimationEvent;
    public bool isGuard = false;
    private float guard = 0.0f;
    [SerializeField]
    private GameObject guardEffect;
    private Collider col;
    int n = 1;

    private void Update()
    {
        if (isGuard)
        {
            guard++;
        }
        if (guard >= 50.0f)
        {
            guard = 0.0f;
            isGuard = false;
        }

        if (SceneManager.GetActiveScene().name != "SoloGameScene")
        {
            enemyColAnim.OnAttackEnded += get_flag =>
            {
                if (get_flag)
                {

                }
                else
                {
                    isGuard = false;
                }
            };
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Attack" || other.tag == "Projectile")
        {
            col = other;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Attack")
        {
            // 上位階層を遡ってColAnimationEventを探す
            Transform currentTransform = other.transform;
            ColAnimationEvent attackerEvent = null;

            while (currentTransform != null)
            {
                attackerEvent = currentTransform.GetComponent<ColAnimationEvent>();
                if (attackerEvent != null)
                {
                    break;
                }
                currentTransform = currentTransform.parent;
            }

            if (attackerEvent != null)
            {
                // 自分自身の攻撃なら無視する
                if (attackerEvent == colAnimationEvent)
                {
                    Debug.Log("自分自身の攻撃なので無視します。");
                    return;
                }
            }

         if (attackerEvent != null && attackerEvent == enemyColAnim)
        {
            isGuard = true;
        }
        }
        else if(other.tag == "Projectile")
        {
            isGuard = true;
        }
    }


    public void InstantiateGuardEffect()
    {
        if(n == 1)
        {
            // 衝突した場所にエフェクトを生成
            Vector3 position = col.ClosestPoint(transform.position); // 衝突した場所の近くの点を取得
            Instantiate(guardEffect, position, Quaternion.identity);
            n = 0;
            StartCoroutine(NUM());
        }
    }

    IEnumerator NUM()
    {
        yield return new WaitForSeconds(0.1f);
        n = 1;
    }

    public bool IsGuard
    {
        get { return isGuard; }
        set { isGuard = value; }
    }
}
