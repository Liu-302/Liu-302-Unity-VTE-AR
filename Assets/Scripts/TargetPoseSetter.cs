using UnityEngine;

public class TargetPoseSetter : MonoBehaviour
{
    [Header("Ŀ���ȹ���")]
    public Transform targetRightUpLeg;
    public Transform targetRightLeg;
    public Transform targetRightFoot;

    public Transform targetLeftUpLeg;
    public Transform targetLeftLeg;
    public Transform targetLeftFoot;

    private Quaternion targetInitialRotation = Quaternion.identity; // �������Ŀ������ĳ�ʼ��ת�븳ֵ����

    private float raiseLegCycle = 2f; // ̧���������ڣ�̧ + ��
    private float raiseLegTimer = 0f;
    private bool isRaisingRightLeg = true;

    private float raiseLegMaxAngle = 45f; // ̧�����Ƕ�

    void Update()
    {
        UpdateRaiseLegTimer();
        UpdateTargetLegPose();
    }

    /// <summary>
    /// ����̧�ȶ�����ʱ���͵�ǰ̧���Ȳ�
    /// </summary>
    private void UpdateRaiseLegTimer()
    {
        raiseLegTimer += Time.deltaTime;

        if (raiseLegTimer > raiseLegCycle)
        {
            raiseLegTimer = 0f;
            isRaisingRightLeg = !isRaisingRightLeg;
        }
    }

    /// <summary>
    /// ���ݵ�ǰ̧�Ȳ࣬����Ŀ����Ϊ���̧����̬����һ����λ
    /// </summary>
    private void UpdateTargetLegPose()
    {
        Quaternion raiseRotation = Quaternion.Euler(raiseLegMaxAngle, 0f, 0f);

        if (isRaisingRightLeg)
        {
            // ����Ŀ�����̧��
            targetRightUpLeg.localRotation = raiseRotation;
            targetRightLeg.localRotation = raiseRotation;
            targetRightFoot.localRotation = raiseRotation;

            // ����Ŀ�������λ
            targetLeftUpLeg.localRotation = targetInitialRotation;
            targetLeftLeg.localRotation = targetInitialRotation;
            targetLeftFoot.localRotation = targetInitialRotation;
        }
        else
        {
            // ����Ŀ�����̧��
            targetLeftUpLeg.localRotation = raiseRotation;
            targetLeftLeg.localRotation = raiseRotation;
            targetLeftFoot.localRotation = raiseRotation;

            // ����Ŀ�������λ
            targetRightUpLeg.localRotation = targetInitialRotation;
            targetRightLeg.localRotation = targetInitialRotation;
            targetRightFoot.localRotation = targetInitialRotation;
        }
    }


}
