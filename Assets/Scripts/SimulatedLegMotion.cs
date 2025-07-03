using UnityEngine;

/// <summary>
/// ʹ�ô��������ݣ�λ�� + ŷ���ǣ�ӳ�䵽����ģ�͵�˫�ȣ�ÿ��������������
/// </summary>
public class SimulatedLegMotion : MonoBehaviour
{
    [Header("Patient����")]
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
        //SimulateAnkleFlexionExtension(); // �׹ؽ�������ϰ��������ͬʱ

        //SimulateAnkleCircumduction(); // �׹ؽڻ�����ϰ��������ͬʱ

        //SimulateRaiseLegAlternately(); // ̧����ϰ�������Ƚ���
        
        //SimulateAnkleDorsiflexionAlternately(); // ����ͷ���ȳ����������ţ��������Ƚ���

        //SimulateQuadricepsIsotonicExercise(); // ����ͷ������������ϰ

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
    /// �׹ؽ�������ϰ������-������������ͬʱ����
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
            float angle = Mathf.Sin(t * 2f * Mathf.PI) * 25f; // ����Ƕȡ�25��

            Quaternion flexionRotation = Quaternion.Euler(angle, 0f, 0f);

            // ���ȡ�С�Ȳ������������춯��
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation * flexionRotation;

            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation * flexionRotation;
        }
        else
        {
            // ��ͣ�ڼ䱣�ֳ�ʼ״̬
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation;

            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation;
        }
    }



    /// <summary>
    /// �׹ؽڻ�����ϰ������ͬʱ��
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

        // ͨ�ýǶ�
        float xAngle = Mathf.Sin(angleDeg * Mathf.Deg2Rad) * 20f;  // ǰ���
        float zAngle = Mathf.Cos(angleDeg * Mathf.Deg2Rad) * 15f;  // ���ⷭ

        // �ҽ�ԭ��
        Quaternion rotRight = Quaternion.Euler(xAngle, 0f, zAngle);
        // ��ž���Z��ȡ��
        Quaternion rotLeft = Quaternion.Euler(xAngle, 0f, -zAngle);

        // Ӧ������
        rightUpLeg.rotation = rightUpLegInitialRotation;
        rightLeg.rotation = rightLegInitialRotation;
        rightFoot.rotation = rightFootInitialRotation * rotRight;

        // Ӧ������
        leftUpLeg.rotation = leftUpLegInitialRotation;
        leftLeg.rotation = leftLegInitialRotation;
        leftFoot.rotation = leftFootInitialRotation * rotLeft;
    }



    /// <summary>
    /// ����ͷ���ȳ��������ű����죩�������Ƚ���
    /// </summary>
    private float dorsiflexCycle = 2f;  // ����һ�� = �� + �ţ���λ���룩
    private float dorsiflexTimer = 0f;
    private bool isRightDorsiflex = true;  // ��ǰ������������ֻ��
    void SimulateAnkleDorsiflexionAlternately()
    {
        dorsiflexTimer += Time.deltaTime;

        float halfCycle = dorsiflexCycle / 2f;
        float angle;

        // ���Ž׶Σ�0 �� max��
        if (dorsiflexTimer <= halfCycle)
        {
            float t = dorsiflexTimer / halfCycle;
            angle = Mathf.Lerp(0f, 20f, t);
        }
        // ���½׶Σ�max �� 0��
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
            // �ҽŹ�
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation * dorsiflexionRotation;

            // ��Ų���
            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation;
        }
        else
        {
            // ��Ź�
            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation * dorsiflexionRotation;

            // �ҽŲ���
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation;
        }
    }


    /// <summary>
    /// ̧����ϰ�������Ƚ���̧��
    /// </summary>
    private float raiseLegCycle = 2f; // ̧���������ڣ�̧ + ��
    private float raiseLegTimer = 0f;
    private bool isRaisingRightLeg = true;

    void SimulateRaiseLegAlternately()
    {
        raiseLegTimer += Time.deltaTime;

        float halfCycle = raiseLegCycle / 2f;
        float angle;

        // ̧��׶�
        if (raiseLegTimer <= halfCycle)
        {
            float t = raiseLegTimer / halfCycle; // 0 ~ 1
            angle = Mathf.Lerp(0f, 45f, t);
        }
        // ���½׶�
        else if (raiseLegTimer <= raiseLegCycle)
        {
            float t = (raiseLegTimer - halfCycle) / halfCycle; // 0 ~ 1
            angle = Mathf.Lerp(45f, 0f, t);
        }
        else
        {
            // һ����ɣ��л��Ȳ����ü�ʱ��
            raiseLegTimer = 0f;
            isRaisingRightLeg = !isRaisingRightLeg;
            return;
        }

        Quaternion raiseRotation = Quaternion.Euler(angle, 0f, 0f);

        if (isRaisingRightLeg)
        {
            // ����������
            rightUpLeg.rotation = rightUpLegInitialRotation * raiseRotation;
            rightLeg.rotation = rightLegInitialRotation * raiseRotation;
            rightFoot.rotation = rightFootInitialRotation * raiseRotation;

            // ���ȱ��־�ֹ
            leftUpLeg.rotation = leftUpLegInitialRotation;
            leftLeg.rotation = leftLegInitialRotation;
            leftFoot.rotation = leftFootInitialRotation;
        }
        else
        {
            // ����������
            leftUpLeg.rotation = leftUpLegInitialRotation * raiseRotation;
            leftLeg.rotation = leftLegInitialRotation * raiseRotation;
            leftFoot.rotation = leftFootInitialRotation * raiseRotation;

            // ���ȱ��־�ֹ
            rightUpLeg.rotation = rightUpLegInitialRotation;
            rightLeg.rotation = rightLegInitialRotation;
            rightFoot.rotation = rightFootInitialRotation;
        }
    }




    /// <summary>
    /// ����ͷ������������ϰ��������У�
    /// </summary>
    private float actionDuration = 1.5f;    // ÿ����������ʱ�䣨��ϥ/̧��/���¶���ʱ�䣩
    private float holdDuration = 0.5f;      // �������ͣ��ʱ�䣨��ֹ�ȴ���
    private float phaseTimer = 0f;          // ��ǰ�׶μ�ʱ��
    private int phase = 0;                  // �����׶� 0~4
    private bool rightLegActive = true;     // ��ǰ��Ծ��ϥ���ȣ�true=���ȣ�false=����

    void SimulateQuadricepsIsotonicExercise()
    {
        phaseTimer += Time.deltaTime;

        // phase3��������ϥ�ȣ���ͣ�٣�ֱ���л�
        float totalPhaseTime = (phase == 3) ? actionDuration : actionDuration + holdDuration;

        if (phaseTimer >= totalPhaseTime)
        {
            phaseTimer = 0f;
            phase++;
            if (phase > 4)
            {
                phase = 0;
                rightLegActive = !rightLegActive; // ����������
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
                case 0: // ������ϥ + ͣ��
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

                case 1: // ����̧�����ȱ�����ϥ
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

                case 2: // ���ȷ��� + ͣ�٣����ȱ�����ϥ
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

                case 3: // ���ȷ��£���ͣ�٣�
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

                case 4: // ��ֹͣ��
                    ResetRightLeg();
                    ResetLeftLeg();
                    break;
            }
        }
        else
        {
            // ������ϥ������̧�ȣ������Ե�
            switch (phase)
            {
                case 0: // ������ϥ + ͣ��
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

                case 1: // ����̧�����ȱ�����ϥ
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

                case 2: // ���ȷ��� + ͣ�٣����ȱ�����ϥ
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

                case 3: // ���ȷ��£���ͣ�٣�
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

                case 4: // ��ֹͣ��
                    ResetRightLeg();
                    ResetLeftLeg();
                    break;
            }
        }
    }


    /// <summary>
    /// ����ʵʱ����������ӳ��
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
    /// ����ʵʱ����������ӳ��
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
    /// ��ť����
    /// </summary>
    public void SetMotionMode(string mode)
    {
        currentMode = mode;
    }
}
