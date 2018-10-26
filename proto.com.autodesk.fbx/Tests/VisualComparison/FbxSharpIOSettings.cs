using UnityEngine;

namespace VisualComparison
{
    public enum SystemUnitType
    {
        mm,
        cm,
        dm,
        m,
        km,
        inch,
        foot,
        yard
    }

    public enum AxisSystemType
    {
        Maya,
        Max,
        Unity
    }

    public class IOSettings : MonoBehaviour
    {
        [Header ("Export Settings")]

        [Tooltip ("Convert to these System Units on export")]
        public SystemUnitType SystemUnit = SystemUnitType.m;

        [Tooltip ("Convert to this Axis System on export")]
        public AxisSystemType AxisSystem = AxisSystemType.Unity;
    }
}
