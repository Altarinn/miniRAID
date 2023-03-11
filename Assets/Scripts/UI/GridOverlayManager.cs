using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace miniRAID.UI
{
    public class GridOverlayManager : MonoBehaviour
    {
        public GridOverlay overlayPrefab;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public GridOverlay FromMoveableRange(Mob mob)
        {
            var moveRange = Databackend.GetSingleton().GetMoveableGrids(mob);
            return FromMoveableRange(mob, moveRange);
        }

        public GridOverlay FromMoveableRange(Mob mob, IEnumerable<Vector2Int> moveRange)
        {
            GridOverlay overlay = Instantiate<GameObject>(overlayPrefab.gameObject, mob.transform.position, Quaternion.identity).GetComponent<GridOverlay>();

            foreach (var p in moveRange)
            {
                overlay.overlay.Add(p, GridOverlay.Types.MOVE);
            }

            overlay.RequestRedraw();
            return overlay;
        }

        public GridOverlay FromDictionary(Dictionary<Vector2Int, GridOverlay.Types> data)
        {
            GridOverlay overlay = Instantiate<GameObject>(overlayPrefab.gameObject, transform.position, Quaternion.identity).GetComponent<GridOverlay>();
            overlay.overlay = data;

            overlay.RequestRedraw();
            return overlay;
        }

        // It WORKS.
        //IEnumerator Coro1(int depth)
        //{
        //    Globals.debugMessage.Instance.Message($"Depth {depth}");
        //    if(depth < 4)
        //    {
        //        yield return new WaitForSeconds(1);
        //        yield return Coro1(depth + 1);
        //        yield return new WaitForSeconds(1);
        //        yield return Coro1(depth + 1);
        //    }
        //    yield break;
        //}
    }
}
