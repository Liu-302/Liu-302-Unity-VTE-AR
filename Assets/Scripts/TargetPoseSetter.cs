using UnityEngine;

public class TargetPoseSetter : MonoBehaviour
{
    [Header("目标腿骨骼")]
    public Transform targetRightUpLeg;
    public Transform targetRightLeg;
    public Transform targetRightFoot;

    public Transform targetLeftUpLeg;
    public Transform targetLeftLeg;
    public Transform targetLeftFoot;

    private Quaternion targetInitialRotation = Quaternion.identity; // 如果你有目标骨骼的初始旋转请赋值这里

    private float raiseLegCycle = 2f; // 抬腿完整周期：抬 + 放
    private float raiseLegTimer = 0f;
    private bool isRaisingRightLeg = true;

    private float raiseLegMaxAngle = 45f; // 抬腿最大角度

    void Update()
    {
        UpdateRaiseLegTimer();
        UpdateTargetLegPose();
    }

    /// <summary>
    /// 更新抬腿动作计时器和当前抬腿腿侧
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
    /// 根据当前抬腿侧，设置目标腿为最大抬腿姿态，另一条复位
    /// </summary>
    private void UpdateTargetLegPose()
    {
        Quaternion raiseRotation = Quaternion.Euler(raiseLegMaxAngle, 0f, 0f);

        if (isRaisingRightLeg)
        {
            // 右腿目标骨骼抬起
            targetRightUpLeg.localRotation = raiseRotation;
            targetRightLeg.localRotation = raiseRotation;
            targetRightFoot.localRotation = raiseRotation;

            // 左腿目标骨骼复位
            targetLeftUpLeg.localRotation = targetInitialRotation;
            targetLeftLeg.localRotation = targetInitialRotation;
            targetLeftFoot.localRotation = targetInitialRotation;
        }
        else
        {
            // 左腿目标骨骼抬起
            targetLeftUpLeg.localRotation = raiseRotation;
            targetLeftLeg.localRotation = raiseRotation;
            targetLeftFoot.localRotation = raiseRotation;

            // 右腿目标骨骼复位
            targetRightUpLeg.localRotation = targetInitialRotation;
            targetRightLeg.localRotation = targetInitialRotation;
            targetRightFoot.localRotation = targetInitialRotation;
        }
    }


}
