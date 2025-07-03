using UnityEngine;

public class LegRenderer : MonoBehaviour
{
    [Header("Line Renderers")]
    public LineRenderer targetRightLegLine;
    public LineRenderer realRightLegLine;
    public LineRenderer targetLeftLegLine;
    public LineRenderer realLeftLegLine;

    [Header("Spheres (脚背、小腿中部、大腿下部、脚尖、脚踝、膝盖、大腿根部)")]
    public Transform[] targetRightLegSpheres = new Transform[7];
    public Transform[] realRightLegSpheres = new Transform[7];
    public Transform[] targetLeftLegSpheres = new Transform[7];
    public Transform[] realLeftLegSpheres = new Transform[7];

    private Vector3[] targetRightPoints = new Vector3[3];
    private Vector3[] realRightPoints = new Vector3[3];
    private Vector3[] targetLeftPoints = new Vector3[3];
    private Vector3[] realLeftPoints = new Vector3[3];

    void Start()
    {
        // 示例初始化
        //targetRightPoints[0] = new Vector3(0.1f, -0.65f, 1.5f);
        //targetRightPoints[1] = new Vector3(0.1f, -0.58f, 1.3f);
        //targetRightPoints[2] = new Vector3(0.1f, -0.4f, 1.0f);

        //realRightPoints[0] = new Vector3(0.1f, -0.75f, 1.5f);
        //realRightPoints[1] = new Vector3(0.1f, -0.65f, 1.3f);
        //realRightPoints[2] = new Vector3(0.1f, -0.43f, 1.0f);

        //targetLeftPoints[0] = new Vector3(-0.1f, -0.65f, 1.5f);
        //targetLeftPoints[1] = new Vector3(-0.1f, -0.58f, 1.3f);
        //targetLeftPoints[2] = new Vector3(-0.1f, -0.4f, 1.0f);

        //realLeftPoints[0] = new Vector3(-0.1f, -0.75f, 1.5f);
        //realLeftPoints[1] = new Vector3(-0.1f, -0.65f, 1.3f);
        //realLeftPoints[2] = new Vector3(-0.1f, -0.43f, 1.0f);

        //抬腿动作
        targetRightPoints[0] = new Vector3(0.2f, -0.70f, 1.45f);
        targetRightPoints[1] = new Vector3(0.2f, -0.75f, 1.25f);
        targetRightPoints[2] = new Vector3(0.2f, -0.75f, 0.93f);

        realRightPoints[0] = new Vector3(0.2f, -0.80f, 1.45f);
        realRightPoints[1] = new Vector3(0.2f, -0.82f, 1.25f);
        realRightPoints[2] = new Vector3(0.2f, -0.82f, 0.93f);

        targetLeftPoints[0] = new Vector3(-0.2f, -0.70f, 1.45f);
        targetLeftPoints[1] = new Vector3(-0.2f, -0.75f, 1.25f);
        targetLeftPoints[2] = new Vector3(-0.2f, -0.75f, 0.93f);

        realLeftPoints[0] = new Vector3(-0.2f, -0.80f, 1.45f);
        realLeftPoints[1] = new Vector3(-0.2f, -0.82f, 1.25f);
        realLeftPoints[2] = new Vector3(-0.2f, -0.82f, 0.93f);

        UpdateLegLines();
    }

    void Update()
    {
        UpdateLegLines();
    }

    void UpdateLegLines()
    {
        UpdateSingleLeg(targetRightLegLine, targetRightLegSpheres, targetRightPoints);
        UpdateSingleLeg(realRightLegLine, realRightLegSpheres, realRightPoints);
        UpdateSingleLeg(targetLeftLegLine, targetLeftLegSpheres, targetLeftPoints);
        UpdateSingleLeg(realLeftLegLine, realLeftLegSpheres, realLeftPoints);
    }

    Vector3 CalculateAnkle(Vector3 footBack, Vector3 shin, Vector3 thighRoot)
    {
        Vector3 footToShin = shin - footBack;
        Vector3 planeNormal = Vector3.Cross(footToShin, thighRoot - shin).normalized;

        float ankleDist = footToShin.magnitude * 0.4f;
        Vector3 ankleBase = Vector3.Lerp(footBack, shin, 0.4f);

        Vector3 footToShinDir = footToShin.normalized;
        Vector3 perpDir = Vector3.Cross(footToShinDir, planeNormal).normalized;

        float bestDiff = float.MaxValue;
        Vector3 bestAnkle = ankleBase;

        float targetAngle1 = Mathf.Deg2Rad * 95f;
        float targetAngle2 = Mathf.Deg2Rad * 105f;

        float maxOffset = ankleDist * 0.5f;
        int steps = 20;
        for (int i = -steps; i <= steps; i++)
        {
            float offset = i * maxOffset / steps;
            Vector3 candidate = ankleBase + perpDir * offset;

            float angle1 = Vector3.Angle(footBack - candidate, shin - candidate) * Mathf.Deg2Rad;
            float angle2 = Vector3.Angle(thighRoot - candidate, footBack - candidate) * Mathf.Deg2Rad;

            float diff = Mathf.Abs(angle1 - targetAngle1) + Mathf.Abs(angle2 - targetAngle2);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                bestAnkle = candidate;
            }
        }
        return bestAnkle;
    }

    void UpdateSingleLeg(LineRenderer line, Transform[] spheres, Vector3[] sensorPoints)
    {
        if (sensorPoints.Length != 3) return;

        Vector3 footBack = sensorPoints[0];
        Vector3 shin = sensorPoints[1];
        Vector3 thigh = sensorPoints[2];

        float thighRootLength = 0.3f;
        Vector3 thighDirGuess = (thigh - shin).normalized;
        Vector3 thighRoot = thigh - thighDirGuess * thighRootLength;
        Vector3 ankle = CalculateAnkle(footBack, shin, thighRoot);

        float kneeExtensionLength = (thigh - shin).magnitude * 0.5f;
        Vector3 ankleToShinDir = (shin - ankle).normalized;
        Vector3 knee = shin + ankleToShinDir * kneeExtensionLength;

        Vector3 thighDir = (knee - thigh).normalized;
        thighRoot = thigh - thighDir * thighRootLength;

        Vector3 footDir = (footBack - ankle).normalized;
        float toeLength = Vector3.Distance(footBack, ankle) * 0.8f;
        Vector3 footToe = footBack + footDir * toeLength;


        if (line != null)
        {
            line.positionCount = 5;
            line.SetPosition(0, thighRoot);
            line.SetPosition(1, knee);
            line.SetPosition(2, ankle);
            line.SetPosition(3, footBack);
            line.SetPosition(4, footToe);
        }

        if (spheres != null && spheres.Length == 7)
        {
            spheres[0].position = footBack;
            spheres[1].position = shin;
            spheres[2].position = thigh;
            spheres[3].position = footToe;
            spheres[4].position = ankle;
            spheres[5].position = knee;
            spheres[6].position = thighRoot;
        }
    }

    // 外部更新接口
    public void SetTargetRightLeg(Vector3 foot, Vector3 shin, Vector3 thigh)
    {
        targetRightPoints[0] = foot;
        targetRightPoints[1] = shin;
        targetRightPoints[2] = thigh;
    }

    public void SetRealRightLeg(Vector3 foot, Vector3 shin, Vector3 thigh)
    {
        realRightPoints[0] = foot;
        realRightPoints[1] = shin;
        realRightPoints[2] = thigh;
    }

    public void SetTargetLeftLeg(Vector3 foot, Vector3 shin, Vector3 thigh)
    {
        targetLeftPoints[0] = foot;
        targetLeftPoints[1] = shin;
        targetLeftPoints[2] = thigh;
    }

    public void SetRealLeftLeg(Vector3 foot, Vector3 shin, Vector3 thigh)
    {
        realLeftPoints[0] = foot;
        realLeftPoints[1] = shin;
        realLeftPoints[2] = thigh;
    }
}

