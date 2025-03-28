using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour
{
    [SerializeField]
    private Hand currentHand;

    private bool isAttack = false;
    private bool isSwing = false;

    private RaycastHit hitInfo;

    void Update(){
        TryAttack();
    }

    public void TryAttack(){
        if(Input.GetButton("Fire1")){
            if(!isAttack){
                StartCoroutine(AttackCoroutine());
            }
        }
    }
    IEnumerator AttackCoroutine(){
        isAttack = true;
        currentHand.anim.SetTrigger("Attack");

        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }
    IEnumerator HitCoroutine(){
        while (isSwing){
            if(CheckObject()){
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }else{

            }
            yield return null;
        }
    }
    private bool CheckObject(){
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range)){
            return true;
        }
        return false;
    }

}
