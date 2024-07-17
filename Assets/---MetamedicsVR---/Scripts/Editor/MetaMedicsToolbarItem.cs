using UnityEditor;
using UnityEngine;

public static class MetaMedicsToolbarItem
{
	const string MENU_ROOT = "MetaMedics/";

	[MenuItem(MENU_ROOT + "Build", false)]
	public static void OpenCsharpSettings() => CenterWindow(EditorWindow.GetWindow<MetaMedicsBuildWindow>("MetaMedics Build"), MetaMedicsBuildWindow.windowWidth, MetaMedicsBuildWindow.windowHeight);

	[MenuItem(MENU_ROOT + "Test", false)]
	public static void OpenImmediateModeDebugger() => CenterWindow(EditorWindow.GetWindow<MetaMedicsTestWindow>("MetaMedics Test"), MetaMedicsTestWindow.windowWidth, MetaMedicsTestWindow.windowHeight);

	private static void CenterWindow(EditorWindow window, int width, int height)
	{
		Vector2 center = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) / 2;
		Vector2 size = new Vector2(width, height);
		window.position = new Rect(center - size / 2, size);
	}
}