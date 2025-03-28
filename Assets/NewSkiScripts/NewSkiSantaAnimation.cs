using UnityEngine;
using System.Collections;


public class NewSkiSantaAnimation : MonoBehaviour
{
    private Transform leftForeArm;
    private Transform rightForeArm;
    private Quaternion leftForeArmDefaultRotation;
    private Quaternion rightForeArmDefaultRotation;

    private ParticleSystem leftPoleTrail;
    private ParticleSystem rightPoleTrail;

    private bool isRotating = false; 

    private void Start()
    {
        leftForeArm = transform.Find("RW_LP_CP_Character_SantaClausRigged/Hips/Spine/Spine1/Spine2/LeftShoulder/LeftArm/LeftForeArm");
        rightForeArm = transform.Find("RW_LP_CP_Character_SantaClausRigged/Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm");


        if (leftForeArm != null) leftForeArmDefaultRotation = leftForeArm.localRotation;
        if (rightForeArm != null) rightForeArmDefaultRotation = rightForeArm.localRotation;

        GameObject leftTrailObj = GameObject.FindWithTag("LeftPoleTrail");
        GameObject rightTrailObj = GameObject.FindWithTag("RightPoleTrail");

        if (leftTrailObj != null) leftPoleTrail = leftTrailObj.GetComponent<ParticleSystem>();
        if (rightTrailObj != null) rightPoleTrail = rightTrailObj.GetComponent<ParticleSystem>();

    }

    public void AdjustForearmRotation(bool isSlowingDown)
    {
        float leftYRotation = isSlowingDown ? -90f : leftForeArmDefaultRotation.eulerAngles.y;
        float rightYRotation = isSlowingDown ? 20f : rightForeArmDefaultRotation.eulerAngles.y;

        Quaternion leftTargetRotation = Quaternion.Euler(leftForeArm.localEulerAngles.x, leftYRotation, leftForeArm.localEulerAngles.z);
        Quaternion rightTargetRotation = Quaternion.Euler(rightForeArm.localEulerAngles.x, rightYRotation, rightForeArm.localEulerAngles.z);

        StopAllCoroutines();
        StartCoroutine(RotateForearms(leftTargetRotation, rightTargetRotation, isSlowingDown));

        if (!isSlowingDown)
        {
            if (leftPoleTrail != null && leftPoleTrail.isPlaying) leftPoleTrail.Stop();
            if (rightPoleTrail != null && rightPoleTrail.isPlaying) rightPoleTrail.Stop();
        }
    }


   private IEnumerator RotateForearms(Quaternion leftTarget, Quaternion rightTarget, bool isSlowingDown)
    {
        float rotationSpeed = 5f; 

        if (isSlowingDown)
        {
            if (leftPoleTrail != null && !leftPoleTrail.isPlaying) leftPoleTrail.Play();
            if (rightPoleTrail != null && !rightPoleTrail.isPlaying) rightPoleTrail.Play();
        }
        
        while (Quaternion.Angle(leftForeArm.localRotation, leftTarget) > 1f || 
            Quaternion.Angle(rightForeArm.localRotation, rightTarget) > 1f)
        {
            leftForeArm.localRotation = Quaternion.Lerp(leftForeArm.localRotation, leftTarget, rotationSpeed * Time.deltaTime);
            rightForeArm.localRotation = Quaternion.Lerp(rightForeArm.localRotation, rightTarget, rotationSpeed * Time.deltaTime);
            yield return null; 
        }

        leftForeArm.localRotation = leftTarget;
        rightForeArm.localRotation = rightTarget;
    }
}
