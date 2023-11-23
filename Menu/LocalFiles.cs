using UnityEngine;
using System.IO;

namespace ShootAR
{
	// LocalFiles는 로컬 파일 작업을 위한 정적 클래스입니다.
	public static class LocalFiles
	{
		/// <summary>
		/// 리소스에서 PersistentDataPath로 파일을 복사합니다.
		/// </summary>
		/// <param name="resource">
		/// 복사할 파일;
		/// 확장자 없이 기본 이름만 제공해야 합니다.
		/// </param>
		/// <param name="targetFile">
		/// 생성할 새 파일;
		/// 기본 이름만 제공해야 합니다.
		/// </param>
		public static void CopyResourceToPersistentData(string resource, string targetFile) {
			// 대상 파일의 경로를 설정합니다.
			targetFile = Path.Combine(Application.persistentDataPath, targetFile);

			// 요청된 파일을 로드합니다.
			TextAsset requestedFile = Resources.Load<TextAsset>(resource);
			// 파일이 리소스에 없으면 예외를 발생시킵니다.
			if (requestedFile == null)
				throw new UnityException($"File not found in Resources: {resource}");

			// 대상 파일에 모든 바이트를 씁니다.
			File.WriteAllBytes(targetFile, requestedFile.bytes);
		}
	}
}

