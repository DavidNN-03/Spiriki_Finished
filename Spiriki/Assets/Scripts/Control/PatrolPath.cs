using UnityEngine;

namespace Spiriki.Control
{
    public class PatrolPath : MonoBehaviour /*This class handles the paths that enemies patrol.*/
    {
        [SerializeField] private const float waypointGizmoRadius = 0.3f; /*Radius of waypoint gizmos.*/
        [SerializeField] private Color waypointGizmoColor = Color.white; /*Color of waypoint gizmos.*/

        private void OnDrawGizmos() /*Draw waypoint gizmos and lines between them.*/
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i).position, waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i).position, GetWaypoint(j).position);
            }
        }

        public int GetNextIndex(int i) /*Returns index of next waypoint.*/
        {
            if (i + 1 == transform.childCount)
            {
                return 0;
            }
            return i + 1;
        }

        public Transform GetWaypoint(int i) /*Returns waypoint transform with given index.*/
        {
            return transform.GetChild(i);
        }
    }
}