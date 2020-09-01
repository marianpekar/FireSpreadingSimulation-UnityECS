namespace MarianPekar.FireSpreadingSimulation
{
    using UnityEngine;

    public class ArrowController : MonoBehaviour
    {
        void LateUpdate()
        {
            var x = Mathf.Cos(GlobalData.Instance.WindDirection * Mathf.Deg2Rad);
            var y = Mathf.Sin(GlobalData.Instance.WindDirection * Mathf.Deg2Rad);

            var target = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + y);
            transform.LookAt(target);

            Debug.DrawLine(transform.position, target, Color.green);
        }
    }
}

