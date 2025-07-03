using UnityEngine;

/// <summary>
/// 使用传感器数据（位置 + 欧拉角）映射到患者模型的双腿，每个动作独立控制
/// </summary>
public class SimulatedLegMotion : MonoBehaviour
{
    [Header("Patient骨骼")]
    public Transform hips;

    public Transform rightUpLeg;
    public Transform rightLeg;
    public Transform rightFoot;

    public Transform leftUpLeg;
    public Transform leftLeg;
    public Transform leftFoot;

    private Quaternion rightUpLegInitialRotation;
    private Quaternion rightLegInitialRotation;
    private Quaternion rightFootInitialRotation;

    private Quaternion leftUpLegInitialRotation;
    private Quaternion leftLegInitialRotation;
    private Quaternion leftFootInitialRotation;

    private string currentMode = "";

    void Start()
    {
        rightUpLegInitialRotation = rightUpLeg.rotation;
        rightLegInitialRotation = rightLeg.rotation;
        rightFootInitialRotation = rightFoot.rotation;

        leftUpLegInitialRotation = leftUpLeg.rotation;
        leftLegInitialRotation = leftLeg.rotation;
        leftFootInitialRotation = leftFoot.rotation;
    }

    void Update()
    {
        //SimulateAnkleFlexionExtension(); // 踝关节屈伸练习，两条腿同时

        //SimulateAnkleCircumduction(); // 踝关节环绕练习，两条腿同时

        //SimulateRaiseLegAlternately(); // 抬腿练习，左右腿交替
        
        //SimulateAnkleDorsiflexionAlternately(); // 股四头肌等长收缩（勾脚），左右腿交替

        //SimulateQuadricepsIsotonicExercise(); // 股四头肌等张收缩练习

        switch (currentMode)
        {
            case "Flexion":
                SimulateAnkleFlexionExtension();
                break;
            case "Circumduction":
                SimulateAnkleCircumduction();
                break;
            case "Raise":
                SimulateRaiseLegAlternately();
                break;
            case "Dorsiflex":
                SimulateAnkleDorsiflexionAlternately();
                break;
            case "Isotonic":
                SimulateQuadricepsIsotonicExercise();
                break;
        }

    }

    private float cycleDuration = 2f;
    private float pauseDuration = 0.5f;
    private float timer = 0f;

    /// <summary>
    /// 踝关节屈伸练习（背屈-跖屈），两腿同时动作
    /// </summary>
    void SimulateAnkleFlexionExtension()
    {
        timer += Time.deltaTime;
        float totalDuration = cycleDuration + pauseDuration;

        if (timer >= totalDuration)
            timer = 0f;

        if (timer < cycleDuration)
        {
            float t = timer / cycleDuration;
            float angle = Mathf.Sin(t * 2f * Mathf.PI) * 25f; // 屈伸角度±25度

            Quaternion flexionRotation = Quaternion.Euler(angle, 0f, 0f);

            // 大腿、小腿不动，脚做屈伸动作
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation * flexionRotation;

            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation * flexionRotation;
        }
        else
        {
            // 暂停期间保持初始状态
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation;

            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation;
        }
    }



    /// <summary>
    /// 踝关节环绕练习，两腿同时做
    /// </summary>
    public float circleDuration = 3f;
    private bool clockwise = true;
    private float elapsedTime = 0f;
    void SimulateAnkleCircumduction()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= circleDuration)
        {
            elapsedTime = 0f;
            clockwise = !clockwise;
        }

        float t = elapsedTime / circleDuration;
        float angleDeg = t * 360f;
        if (!clockwise) angleDeg = -angleDeg;

        // 通用角度
        float xAngle = Mathf.Sin(angleDeg * Mathf.Deg2Rad) * 20f;  // 前后摆
        float zAngle = Mathf.Cos(angleDeg * Mathf.Deg2Rad) * 15f;  // 内外翻

        // 右脚原样
        Quaternion rotRight = Quaternion.Euler(xAngle, 0f, zAngle);
        // 左脚镜像：Z轴取反
        Quaternion rotLeft = Quaternion.Euler(xAngle, 0f, -zAngle);

        // 应用右腿
        rightUpLeg.rotation = rightUpLegInitialRotation;
        rightLeg.rotation = rightLegInitialRotation;
        rightFoot.rotation = rightFootInitialRotation * rotRight;

        // 应用左腿
        leftUpLeg.rotation = leftUpLegInitialRotation;
        leftLeg.rotation = leftLegInitialRotation;
        leftFoot.rotation = leftFootInitialRotation * rotLeft;
    }



    /// <summary>
    /// 股四头肌等长收缩（脚背屈伸），左右腿交替
    /// </summary>
    private float dorsiflexCycle = 2f;  // 完整一轮 = 勾 + 放（单位：秒）
    private float dorsiflexTimer = 0f;
    private bool isRightDorsiflex = true;  // 当前做动作的是哪只脚
    void SimulateAnkleDorsiflexionAlternately()
    {
        dorsiflexTimer += Time.deltaTime;

        float halfCycle = dorsiflexCycle / 2f;
        float angle;

        // 勾脚阶段（0 → max）
        if (dorsiflexTimer <= halfCycle)
        {
            float t = dorsiflexTimer / halfCycle;
            angle = Mathf.Lerp(0f, 20f, t);
        }
        // 放下阶段（max → 0）
        else if (dorsiflexTimer <= dorsiflexCycle)
        {
            float t = (dorsiflexTimer - halfCycle) / halfCycle;
            angle = Mathf.Lerp(20f, 0f, t);
        }
        else
        {
            dorsiflexTimer = 0f;
            isRightDorsiflex = !isRightDorsiflex;
            return;
        }

        Quaternion dorsiflexionRotation = Quaternion.Euler(angle, 0f, 0f);

        if (isRightDorsiflex)
        {
            // 右脚勾
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation * dorsiflexionRotation;

            // 左脚不动
            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation;
        }
        else
        {
            // 左脚勾
            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation * dorsiflexionRotation;

            // 右脚不动
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation;
        }
    }


    /// <summary>
    /// 抬腿练习，左右腿交替抬高
    /// </summary>
    private float raiseLegCycle = 2f; // 抬腿完整周期：抬 + 放
    private float raiseLegTimer = 0f;
    private bool isRaisingRightLeg = true;

    void SimulateRaiseLegAlternately()
    {
        raiseLegTimer += Time.deltaTime;

        float halfCycle = raiseLegCycle / 2f;
        float angle;

        // 抬起阶段
        if (raiseLegTimer <= halfCycle)
        {
            float t = raiseLegTimer / halfCycle; // 0 ~ 1
            angle = Mathf.Lerp(0f, 45f, t);
        }
        // 放下阶段
        else if (raiseLegTimer <= raiseLegCycle)
        {
            float t = (raiseLegTimer - halfCycle) / halfCycle; // 0 ~ 1
            angle = Mathf.Lerp(45f, 0f, t);
        }
        else
        {
            // 一轮完成：切换腿并重置计时器
            raiseLegTimer = 0f;
            isRaisingRightLeg = !isRaisingRightLeg;
            return;
        }

        Quaternion raiseRotation = Quaternion.Euler(angle, 0f, 0f);

        if (isRaisingRightLeg)
        {
            // 右腿做动作
            rightUpLeg.rotation = rightUpLegInitialRotation * raiseRotation;
            rightLeg.rotation = rightLegInitialRotation * raiseRotation;
            rightFoot.rotation = rightFootInitialRotation * raiseRotation;

            // 左腿保持静止
            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation;
        }
        else
        {
            // 左腿做动作
            leftUpLeg.rotation = leftUpLegInitialRotation * raiseRotation;
            leftLeg.rotation = leftLegInitialRotation * raiseRotation;
            leftFoot.rotation = leftFootInitialRotation * raiseRotation;

            // 右腿保持静止
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation;
        }
    }




    /// <summary>
    /// 股四头肌等张收缩练习（交替进行）
    /// </summary>
    private float actionDuration = 1.5f;    // 每个动作持续时间（屈膝/抬腿/放下动作时间）
    private float holdDuration = 0.5f;      // 动作后的停顿时间（静止等待）
    private float phaseTimer = 0f;          // 当前阶段计时器
    private int phase = 0;                  // 动作阶段 0~4
    private bool rightLegActive = true;     // 当前活跃屈膝的腿，true=右腿，false=左腿

    void SimulateQuadricepsIsotonicExercise()
    {
        phaseTimer += Time.deltaTime;

        // phase3（放下屈膝腿）无停顿，直接切换
        float totalPhaseTime = (phase == 3) ? actionDuration : actionDuration + holdDuration;

        if (phaseTimer >= totalPhaseTime)
        {
            phaseTimer = 0f;
            phase++;
            if (phase > 4)
            {
                phase = 0;
                rightLegActive = !rightLegActive; // 交替左右腿
            }
        }

        float t = Mathf.Clamp01(phaseTimer / actionDuration);

        Quaternion thighRaiseRot = Quaternion.Euler(45f, 0f, 0f);
        Quaternion calfFoldRot = Quaternion.Euler(-70f, 0f, 0f);
        Quaternion footFlatRot = Quaternion.Euler(-90f, 0f, 0f);

        void ResetRightLeg()
        {
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation;
        }

        void ResetLeftLeg()
        {
            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation;
        }

        if (rightLegActive)
        {
            switch (phase)
            {
                case 0: // 右腿屈膝 + 停顿
                    if (phaseTimer <= actionDuration)
                    {
                        rightUpLeg.rotation = Quaternion.Slerp(rightUpLegInitialRotation, rightUpLegInitialRotation * thighRaiseRot, t);
                        rightLeg.rotation = Quaternion.Slerp(rightLegInitialRotation, rightLegInitialRotation * calfFoldRot, t);
                        rightFoot.rotation = Quaternion.Slerp(rightFootInitialRotation, rightFootInitialRotation * footFlatRot, t);
                    }
                    else
                    {
                        rightUpLeg.rotation = rightUpLegInitialRotation * thighRaiseRot;
                        rightLeg.rotation = rightLegInitialRotation * calfFoldRot;
                        rightFoot.rotation = rightFootInitialRotation * footFlatRot;
                    }
                    ResetLeftLeg();
                    break;

                case 1: // 左腿抬起，右腿保持屈膝
                    rightUpLeg.rotation = rightUpLegInitialRotation * thighRaiseRot;
                    rightLeg.rotation = rightLegInitialRotation * calfFoldRot;
                    rightFoot.rotation = rightFootInitialRotation * footFlatRot;

                    if (phaseTimer <= actionDuration)
                    {
                        Quaternion raiseRot = Quaternion.Euler(Mathf.Lerp(0f, 45f, t), 0f, 0f);
                        leftUpLeg.rotation = leftUpLegInitialRotation * raiseRot;
                        leftLeg.rotation = leftLegInitialRotation * raiseRot;
                        leftFoot.rotation = leftFootInitialRotation * raiseRot;
                    }
                    else
                    {
                        Quaternion raiseRot = Quaternion.Euler(45f, 0f, 0f);
                        leftUpLeg.rotation = leftUpLegInitialRotation * raiseRot;
                        leftLeg.rotation = leftLegInitialRotation * raiseRot;
                        leftFoot.rotation = leftFootInitialRotation * raiseRot;
                    }
                    break;

                case 2: // 左腿放下 + 停顿，右腿保持屈膝
                    rightUpLeg.rotation = rightUpLegInitialRotation * thighRaiseRot;
                    rightLeg.rotation = rightLegInitialRotation * calfFoldRot;
                    rightFoot.rotation = rightFootInitialRotation * footFlatRot;

                    if (phaseTimer <= actionDuration)
                    {
                        Quaternion lowerRot = Quaternion.Euler(Mathf.Lerp(45f, 0f, t), 0f, 0f);
                        leftUpLeg.rotation = leftUpLegInitialRotation * lowerRot;
                        leftLeg.rotation = leftLegInitialRotation * lowerRot;
                        leftFoot.rotation = leftFootInitialRotation * lowerRot;
                    }
                    else
                    {
                        ResetLeftLeg();
                    }
                    break;

                case 3: // 右腿放下（无停顿）
                    if (phaseTimer <= actionDuration)
                    {
                        rightUpLeg.rotation = Quaternion.Slerp(rightUpLegInitialRotation * thighRaiseRot, rightUpLegInitialRotation, t);
                        rightLeg.rotation = Quaternion.Slerp(rightLegInitialRotation * calfFoldRot, rightLegInitialRotation, t);
                        rightFoot.rotation = Quaternion.Slerp(rightFootInitialRotation * footFlatRot, rightFootInitialRotation, t);
                    }
                    else
                    {
                        ResetRightLeg();
                    }
                    ResetLeftLeg();
                    break;

                case 4: // 静止停顿
                    ResetRightLeg();
                    ResetLeftLeg();
                    break;
            }
        }
        else
        {
            // 左腿屈膝，右腿抬腿；动作对调
            switch (phase)
            {
                case 0: // 左腿屈膝 + 停顿
                    if (phaseTimer <= actionDuration)
                    {
                        leftUpLeg.rotation = Quaternion.Slerp(leftUpLegInitialRotation, leftUpLegInitialRotation * thighRaiseRot, t);
                        leftLeg.rotation = Quaternion.Slerp(leftLegInitialRotation, leftLegInitialRotation * calfFoldRot, t);
                        leftFoot.rotation = Quaternion.Slerp(leftFootInitialRotation, leftFootInitialRotation * footFlatRot, t);
                    }
                    else
                    {
                        leftUpLeg.rotation = leftUpLegInitialRotation * thighRaiseRot;
                        leftLeg.rotation = leftLegInitialRotation * calfFoldRot;
                        leftFoot.rotation = leftFootInitialRotation * footFlatRot;
                    }
                    ResetRightLeg();
                    break;

                case 1: // 右腿抬起，左腿保持屈膝
                    leftUpLeg.rotation = leftUpLegInitialRotation * thighRaiseRot;
                    leftLeg.rotation = leftLegInitialRotation * calfFoldRot;
                    leftFoot.rotation = leftFootInitialRotation * footFlatRot;

                    if (phaseTimer <= actionDuration)
                    {
                        Quaternion raiseRot = Quaternion.Euler(Mathf.Lerp(0f, 45f, t), 0f, 0f);
                        rightUpLeg.rotation = rightUpLegInitialRotation * raiseRot;
                        rightLeg.rotation = rightLegInitialRotation * raiseRot;
                        rightFoot.rotation = rightFootInitialRotation * raiseRot;
                    }
                    else
                    {
                        Quaternion raiseRot = Quaternion.Euler(45f, 0f, 0f);
                        rightUpLeg.rotation = rightUpLegInitialRotation * raiseRot;
                        rightLeg.rotation = rightLegInitialRotation * raiseRot;
                        rightFoot.rotation = rightFootInitialRotation * raiseRot;
                    }
                    break;

                case 2: // 右腿放下 + 停顿，左腿保持屈膝
                    leftUpLeg.rotation = leftUpLegInitialRotation * thighRaiseRot;
                    leftLeg.rotation = leftLegInitialRotation * calfFoldRot;
                    leftFoot.rotation = leftFootInitialRotation * footFlatRot;

                    if (phaseTimer <= actionDuration)
                    {
                        Quaternion lowerRot = Quaternion.Euler(Mathf.Lerp(45f, 0f, t), 0f, 0f);
                        rightUpLeg.rotation = rightUpLegInitialRotation * lowerRot;
                        rightLeg.rotation = rightLegInitialRotation * lowerRot;
                        rightFoot.rotation = rightFootInitialRotation * lowerRot;
                    }
                    else
                    {
                        ResetRightLeg();
                    }
                    break;

                case 3: // 左腿放下（无停顿）
                    if (phaseTimer <= actionDuration)
                    {
                        leftUpLeg.rotation = Quaternion.Slerp(leftUpLegInitialRotation * thighRaiseRot, leftUpLegInitialRotation, t);
                        leftLeg.rotation = Quaternion.Slerp(leftLegInitialRotation * calfFoldRot, leftLegInitialRotation, t);
                        leftFoot.rotation = Quaternion.Slerp(leftFootInitialRotation * footFlatRot, leftFootInitialRotation, t);
                    }
                    else
                    {
                        ResetLeftLeg();
                    }
                    ResetRightLeg();
                    break;

                case 4: // 静止停顿
                    ResetRightLeg();
                    ResetLeftLeg();
                    break;
            }
        }
    }


    /// <summary>
    /// 右腿实时传感器数据映射
    /// </summary>
    public void UpdateLegData(
        Vector3 upLegLocalPos, Vector3 upLegEulerAngles,
        Vector3 legLocalPos, Vector3 legEulerAngles,
        Vector3 footLocalPos, Vector3 footEulerAngles)
    {
        if (hips == null) return;

        rightUpLeg.position = hips.position + upLegLocalPos;
        rightLeg.position = hips.position + legLocalPos;
        rightFoot.position = hips.position + footLocalPos;

        rightUpLeg.rotation = hips.rotation * Quaternion.Euler(upLegEulerAngles);
        rightLeg.rotation = hips.rotation * Quaternion.Euler(legEulerAngles);
        rightFoot.rotation = hips.rotation * Quaternion.Euler(footEulerAngles);
    }

    /// <summary>
    /// 左腿实时传感器数据映射
    /// </summary>
    public void UpdateLeftLegData(
        Vector3 upLegLocalPos, Vector3 upLegEulerAngles,
        Vector3 legLocalPos, Vector3 legEulerAngles,
        Vector3 footLocalPos, Vector3 footEulerAngles)
    {
        if (hips == null) return;

        leftUpLeg.position = hips.position + upLegLocalPos;
        leftLeg.position = hips.position + legLocalPos;
        leftFoot.position = hips.position + footLocalPos;

        leftUpLeg.rotation = hips.rotation * Quaternion.Euler(upLegEulerAngles);
        leftLeg.rotation = hips.rotation * Quaternion.Euler(legEulerAngles);
        leftFoot.rotation = hips.rotation * Quaternion.Euler(footEulerAngles);
    }


    /// <summary>
    /// 按钮设置
    /// </summary>
    public void SetMotionMode(string mode)
    {
        currentMode = mode;
    }
}
