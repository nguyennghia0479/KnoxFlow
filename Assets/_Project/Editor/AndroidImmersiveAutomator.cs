#if UNITY_EDITOR && UNITY_ANDROID
using System.IO;
using UnityEditor.Android;
using UnityEngine;

public class AndroidImmersiveAutomator : IPostGenerateGradleAndroidProject
{
    // Thứ tự thực thi script khi build (số càng nhỏ chạy càng sớm)
    public int callbackOrder => 99;

    public void OnPostGenerateGradleAndroidProject(string basePath)
    {
        // 1. TỰ ĐỘNG TẠO FILE JAVA NATIVE
        // Đường dẫn đến thư mục chứa mã nguồn Java trong project Gradle vừa sinh ra
        string javaPackagePath = Path.Combine(basePath, "src", "main", "java", "com", "unity3d", "player");
        if (!Directory.Exists(javaPackagePath))
        {
            Directory.CreateDirectory(javaPackagePath);
        }

        string javaFilePath = Path.Combine(javaPackagePath, "CustomGameActivity.java");
        string javaCode = @"package com.unity3d.player;
import android.os.Bundle;
import android.view.View;
import android.view.Window;
import android.view.WindowInsets;
import android.view.WindowInsetsController;

public class CustomGameActivity extends UnityPlayerGameActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        applyStrictImmersive();
        View decorView = getWindow().getDecorView();
        if (decorView != null) {
            decorView.setOnApplyWindowInsetsListener((v, insets) -> {
                applyStrictImmersive();
                return WindowInsets.CONSUMED;
            });
        }
    }
    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        if (hasFocus) applyStrictImmersive();
    }
    @Override
    protected void onResume() {
        super.onResume();
        applyStrictImmersive();
    }
    private void applyStrictImmersive() {
        final Window window = getWindow();
        if (window == null) return;
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                window.setDecorFitsSystemWindows(false);
                WindowInsetsController controller = window.getInsetsController();
                if (controller != null) {
                    controller.setSystemBarsBehavior(WindowInsetsController.BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE);
                    controller.hide(WindowInsets.Type.systemBars());
                }
            }
        });
    }
}";
        File.WriteAllText(javaFilePath, javaCode);

        // 2. TỰ ĐỘNG SỬA MANIFEST TRONG QUÁ TRÌNH BUILD
        string manifestPath = Path.Combine(basePath, "src", "main", "AndroidManifest.xml");
        if (File.Exists(manifestPath))
        {
            string manifestContent = File.ReadAllText(manifestPath);

            // Tìm và thay thế class gọi Activity mặc định sang Class Custom Java của chúng ta
            if (manifestContent.Contains("com.unity3d.player.UnityPlayerGameActivity"))
            {
                manifestContent = manifestContent.Replace(
                    "com.unity3d.player.UnityPlayerGameActivity",
                    "com.unity3d.player.CustomGameActivity"
                );
                File.WriteAllText(manifestPath, manifestContent);
                Debug.Log("<color=green>[Immersive Automator]</color> Auto-injected CustomGameActivity and patched AndroidManifest successfully!");
            }
        }
    }
}
#endif