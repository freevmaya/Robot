using System.Collections.Generic;
using UnityEngine;
using Vmaya.Robot;
using Vmaya.Scene3D;

public class IKSystem : MonoBehaviour
{
    public static float MIN_DELTA = 0.001f;
    private IRotationElement[] _nodes;

    private void Start()
    {
        getElements();
    }

    private void getElements()
    {
        _nodes = GetComponentsInChildren<IRotationElement>();
    }

    public int indexOf(IRotationElement elem)
    {
        for (int i = 0; i < _nodes.Length; i++)
            if (_nodes[i] == elem) return i;

        return -1;
    }

    internal static List<float> getLengths(List<Vector3> points)
    {
        List<float> _lengths = new List<float>();

        for (int i = 1; i < points.Count; i++)
            _lengths.Add((points[i - 1] - points[i]).magnitude);

        return _lengths;
    }
    private static float calcAngle(List<Vector3> p, int idx, int inc, ref Vector3 axis)
    {
        int lidx = idx - inc;
        int ridx = idx + inc;
        if (lidx < 0) return 180;
        else if (ridx > p.Count - 1) return 180;
        else
        {
            Vector3 v1 = p[lidx] - p[idx];
            Vector3 v2 = p[ridx] - p[idx];

            //axis = Vector3.Cross(v1, v2);
            return Vector3.SignedAngle(v1, v2, axis);
        }
    }

    internal void Fabrik(Vector3 delta, Transform transform)
    {
        IRotationElement re = transform.GetComponent<IRotationElement>();
        int idx = indexOf(re);
        if (idx > -1)
        {
            List<Vector3> result = Fabrik(_nodes, delta, idx, 0);
        }
    }

    public static List<Vector3> Fabrik(IRotationElement[] _nodes, Vector3 a_delta, int sidx, int eidx)
    {

        int def = eidx - sidx;      // –азница между первым и последним узлам
        int count = Mathf.Abs(def); // ¬сего узлов
        int inc = def > 0 ? 1 : -1; // »нкремент прохождени€ узлов
        int dixl = def < 0 ? 1 : 0; // ƒл€ вычислени€ индекса длины плеча

        List<Vector3> p = new List<Vector3>();
        List<float> lengths = new List<float>();

        Vector3 angleLimit(int ix, int inc, Vector3 a_direct, float min, float max)
        {
            Vector3 planeNormal = _nodes[ix].getAxis();
            int ainc = Mathf.Abs(inc);
            if ((max > 0) && (ix < p.Count - ainc) && (ix >= ainc))
            {
                float limitDelta = Mathf.Max(max - min, 1);

                Vector3 axis = planeNormal;
                float angle = calcAngle(p, ix, inc, ref axis);
                float absAngle = Mathf.Abs(angle);
                if (absAngle < max)
                {
                    float delta = Mathf.Pow(Mathf.Min((max - absAngle) / limitDelta, 1), 2) * limitDelta;// * animSmooth;
                    if (angle > 0)
                        a_direct = Quaternion.AngleAxis(delta, planeNormal) * a_direct;
                    else a_direct = Quaternion.AngleAxis(-delta, planeNormal) * a_direct;
                }
            }
            return a_direct;
        }

        void reversePass(int ri)
        {
            //Ётап 2 : обратное следование, если требуетс€

            while (ri != eidx)
            {
                int nid = ri + inc;
                Vector3 direct = (p[nid] - p[ri]).normalized;
                Limit limit = _nodes[ri].getAngleLimiter();
                p[nid] = p[ri] + angleLimit(ri, inc, direct, limit.min, limit.max) * lengths[ri - dixl];
                //p[nid] = p[ri] + direct * a_lengths[ri - dixl];
                ri += inc;
            }
        }

        bool calcBackCorrect(int ix, Vector3 newPos, ref Vector3 backPoint)
        {
            int bix = ix - inc;
            int nix = ix + inc;
            Limit limit = _nodes[ix].getAngleLimiter();
            if ((limit.max > 0) && (bix >= 0) && (bix < p.Count) && (nix >= 0) && (nix < p.Count))
            {
                Vector3 planeNormal = _nodes[ix].getAxis();

                float limitDelta = Mathf.Max(limit.max - limit.min, 1);
                Vector3 direct = (p[ix] - p[nix]).normalized;
                Vector3 backDirect = (newPos - p[bix]).normalized * lengths[ix];

                float angle = Vector3.SignedAngle(direct, backDirect, planeNormal);
                float absAngle = Mathf.Abs(angle);
                if (absAngle < limit.max)
                {
                    float delta = Mathf.Pow(Mathf.Min((limit.max - absAngle) / limitDelta, 1), 2) * limitDelta;// * animSmooth;

                    Quaternion q;
                    if (angle > 0)
                        q = Quaternion.AngleAxis(delta, planeNormal);
                    else q = Quaternion.AngleAxis(-delta, planeNormal);
                    backPoint = p[ix] - q * backDirect;
                    return true;
                }
            }
            return false;
        }

        foreach (IRotationElement node in _nodes)
            p.Add(node.GetPosition());

        lengths = getLengths(p);

        Vector3 origT = p[eidx] + a_delta;

        float distNodes = 0;

        int idx = sidx;
        int i;

        do { distNodes += lengths[idx - dixl]; idx += inc; } while (idx != eidx);

        /*
        if (dist > distNodes)
        {
            //цель недостижима

            direct = (t - p[sidx]).normalized;
            t = p[sidx] + direct * distNodes;
        }*/

        float tol = MIN_DELTA * distNodes / count;

        int brk = 0;
        Vector3 newPos;

        do
        {

            //Ётап 1 : пр€мое следование
            //ѕытаемс€ устанавить конечный узел p[eidx] на позицию цели

            Vector3 backPoint = default;

            newPos = origT;
            i = eidx;

            if (calcBackCorrect(i, newPos, ref backPoint))
            {
                p[eidx] = newPos;
                p[i - inc] = backPoint;
            }
            else p[eidx] = newPos;

            do
            {
                i -= inc;

                //Ётап 2 : обратное следование
                reversePass(i);

            } while (i != sidx);

            brk++;

        } while (((p[eidx] - origT).sqrMagnitude > tol) && (brk < 2));

        return p;
    }

    //Ѕазовый алгоритм по публикации http://andreasaristidou.com/publications/FABRIK.pdf перевод https://habr.com/ru/post/222689/ 
    //Ѕез учета плоскостей поворота узлов и лимитов 
    public static void BaseFabrik(ref Vector3[] p, Vector3 t, float tol = 0.1f)
    {
        int n = p.Length - 1;

        float[] d = new float[n];
        float[] r = new float[n];
        float fullDist = 0;
        for (int i = 0; i < n; i++)
            fullDist += d[i] = (p[i] - p[i + 1]).magnitude;

        float dist = (p[n] - t).magnitude;


        if (dist > fullDist)
        { //цель недостижима
            for (int i = 0; i < n; i++)
            {
                //Ќайдем дистанцию r[i] между целью t и узлом p[i]

                r[i] = (p[i] - t).magnitude;
                float lambda = d[i] / r[i];

                //Ќаходим новую позицию узла p[i]
                p[i + 1] = (1 - lambda) * p[i] + lambda * t;
            }
        }
        else
        {
            //ƒель достижима; т.о. b будет новой позицией узла p[1]
            Vector3 b = p[0];
            float lambda;

            //ѕровер€ем, не выше ли дистанци€ между конечным узлом p[n] и 
            //целевой позицией t значени€ терпимости (tolerance)

            float DIFa = (p[n] - t).magnitude;
            int count = 0;
            while ((DIFa > tol) && (count < 100))
            {

                //Ётап 1 : пр€мое следование
                //”станавливаем конечный узел p[n] в качестве цели (веро€тно, имелось ввиду "ставим на позицию цели" - прим. перев.)

                p[n] = t;
                for (int i = n - 1; i >= 0; i--)
                {
                    //ѕолучаем рассто€ние r[i] между узлом p[i] и новой позицией p[i+1]

                    r[i] = (p[i + 1] - p[i]).magnitude;

                    lambda = d[i] / r[i];

                    //ѕолучаем новую позицию p[i]

                    p[i] = (1 - lambda) * p[i + 1] + lambda * p[i];
                }

                //Ётап 2: обратное следование
                //”станавливаем корневому элементу p[1] начальную позицию

                p[0] = b;
                for (int i = 0; i < n; i++)
                {
                    //ѕолучаем дистанцию r[i] между узлом p[i+1] и позицией p[i]

                    r[i] = (p[i + 1] - p[i]).magnitude;

                    lambda = d[i] / r[i];

                    //ѕолучаем новую позицию p[i]

                    p[i + 1] = (1 - lambda) * p[i] + lambda * p[i + 1];
                }

                DIFa = (p[n] - t).magnitude;
                count++;
            }
        }
    }

    public static float[] SimpleFabrik2(ref Vector3[] p, Vector3[] axis, float[,] limits, Vector3 t, Vector3 baseDirect, float tol = 0.1f)
    {
        Vector3[] direct = new Vector3[p.Length];

        int n = p.Length - 1;

        float[] d = new float[n];
        float[] angles = new float[n];
        for (int i = 0; i < n; i++) d[i] = (p[i] - p[i + 1]).magnitude;

        Vector3 b = p[0];


        float DIFa;
        int count = 0;
        do
        {

            p[n] = t;
            for (int i = n - 1; i >= 0; i--)
            {
                direct[i] = p[i] - p[i + 1];

                if (axis != null) direct[i] = Vector3.ProjectOnPlane(direct[i], axis[i]);

                direct[i] = direct[i].normalized;

                p[i] = p[i + 1] + direct[i] * d[i];
            }

            p[0] = b;
            for (int i = 0; i < n; i++)
            {
                Vector3 dir = direct[i];

                Vector3 baseVector = (i > 0) ? (p[i - 1] - p[i]).normalized : -baseDirect;

                Vector3 aaxis = ((axis != null) ? axis[i] : Vector3.Cross(baseVector, dir)).normalized;
                if (aaxis.sqrMagnitude == 0) aaxis = Vector3.left;

                float angle = Vector3.SignedAngle(baseVector, dir, aaxis);

                if ((limits != null) && (limits[i, 0] != limits[i, 1]))
                {
                    angle = Mathf.Clamp(angle, limits[i, 0], limits[i, 1]);
                    dir = Quaternion.AngleAxis(angle, aaxis) * baseVector;
                }

                angles[i] = angle;
                p[i + 1] = p[i] - dir * d[i];
            }

            DIFa = (p[n] - t).magnitude;
            count++;
        } while ((DIFa > tol) && (count < 20));

        Debug.Log(count);
        return angles;
    }

    public static Vector3[] NodesFabrik(List<IRotationElement> nodes, int start, int end, out float[] angles, Vector3 t, float tolerance = 0.1f)
    {
        int n = Mathf.Min(end, nodes.Count - 1);
        Vector3[] p = new Vector3[nodes.Count];
        Vector3[] axis = new Vector3[n];

        angles = new float[n];

        for (int i = start; i <= n; i++)
            p[i] = nodes[i].GetPosition();

        for (int i = start; i < n; i++)
            axis[i] = nodes[i].getAxis();

        for (int i = start; i < n; i++)
            angles[i] = nodes[i].getAngle();

        float[] d = new float[n];
        for (int i = 0; i < n; i++) d[i] = (p[i] - p[i + 1]).magnitude;

        Vector3 b = p[0];
        Vector3 dir;

        int maxCycles = 1;
        int count = 0;
        float diff;

        do
        {

            p[n] = t;
            for (int i = n - 1; i >= start; i--)
            {
                dir = Vector3.ProjectOnPlane(p[i + 1] - p[i], axis[i]).normalized;
                p[i] = p[i + 1] - dir * d[i];
            }

            p[0] = b;
            for (int i = start; i < n; i++)
            {
                dir = Vector3.ProjectOnPlane(p[i + 1] - p[i], axis[i]).normalized;

                float angle = nodes[i].getAngleLimiter().Clamp(Vector3.SignedAngle(nodes[i].getBaseDirect(), dir, axis[i]));
                float delta = angle - angles[i];
                Quaternion deltaQ = Quaternion.AngleAxis(delta, axis[i]);
                for (int a = i + 1; a < n; a++)
                    axis[a] = deltaQ * axis[a];
                angles[i] = angle;

                p[i + 1] = p[i] + dir * d[i];
            }

            diff = (t - p[n]).magnitude;
            count++;

        } while ((diff > tolerance) && (count < maxCycles));

        Debug.Log(diff + " " + count);

        return p;
    }
}
