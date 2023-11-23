using UnityEngine;

namespace ShootAR
{
    /* Orbit 구조체: 특정 중심점(centerPoint)을 중심으로 주어진
	 방향(direction)과 수직한 축(perpendicularAxis)을 가지는 궤도를 정의합니다.*/
    public struct Orbit
    {
		public Vector3 direction, centerPoint, perpendicularAxis;
        // 주어진 방향으로 중심, 궤도의 중심점, 수직한 축을 초기화하는 생성자입니다.
        public Orbit(Vector3 direction, Vector3 centerPoint, Vector3 perpendicularAxis)
        {
            this.direction = direction;
            this.centerPoint = centerPoint;
            this.perpendicularAxis = perpendicularAxis;
        }

        /// <summary>
        /// <paramref name="point"/>의 크기를 반지름으로 하는 <paramref name="centerPoint"/> 주변의 원형 궤도를 생성합니다.
        /// </summary>
        /// <param name="point">궤도 상의 한 지점</param>
        /// <param name="centerPoint">궤도의 중심</param>
        /// <param name="clockwise">궤도의 방향. true = 시계 방향; 기본값은 true</param>
        public Orbit(Vector3 point, Vector3 centerPoint, bool clockwise = true)
        {
            // 궤도의 방향은 중심에서 지점으로 향하는 벡터입니다.
            direction = centerPoint - point;
            this.centerPoint = centerPoint;

            // 수직한 축은 주어진 방향 벡터의 중간을 잡고, 시계 방향이면 왼쪽, 그렇지 않으면 오른쪽으로 교차합니다.
            perpendicularAxis = Vector3.Cross(direction / 2, clockwise ? Vector3.left : Vector3.right);
        }
    }
}

