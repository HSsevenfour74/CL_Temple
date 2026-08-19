using UnityEngine;
using System.IO;
using System.Globalization;
using UnityEngine.Rendering.PostProcessing;
using ConceptGames.ConceptLineOrion.Level;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PostProcessManager : MonoBehaviour
{
    public static PostProcessManager Instance;
    public Camera targetCamera;
    
    [Header("Settings")]
    public string editorFolder = "Custom";
    public string buildFolder = "PostConfigs";
    public string defaultFileName = "DefaultPostConfig";
    public string extension = "bytes";

    private PostProcessVolume volume;

    private void Awake()
    {
        Instance = this;
    }

    private string GetConfigPath(string fileName)
    {
        string root;
#if UNITY_EDITOR
        // 编辑器模式：指向 Assets/Custom
        root = Path.Combine(Application.dataPath, editorFolder);
#else
        // 打包模式：指向 StreamingAssets/PostConfigs
        root = Path.Combine(Application.streamingAssetsPath, buildFolder);
#endif
        return Path.Combine(root, fileName + "." + extension);
    }

    public void SyncByLevel(LevelData id)
    {
        string levelName = id.ToString();
        string path = GetConfigPath(levelName);

        if (File.Exists(path))
        {
            SyncFromConfig(levelName);
        }
        else
        {
            string defaultPath = GetConfigPath(defaultFileName);
            if (File.Exists(defaultPath))
                SyncFromConfig(defaultFileName);
            else
                ClearProfile();
        }
    }

    public void ClearProfile()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null) return;

        volume = targetCamera.GetComponent<PostProcessVolume>();
        if (volume != null)
        {
            if (volume.profile != null && volume.profile.name.Contains("Temp_"))
            {
                Destroy(volume.profile);
            }
            volume.profile = null;
            volume.enabled = false;
        }
    }

    public void SyncFromConfig(string fileNameToLoad)
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null) return;

        string path = GetConfigPath(fileNameToLoad);
        if (!File.Exists(path))
        {
            ClearProfile();
            return;
        }

        volume = targetCamera.GetComponent<PostProcessVolume>() ?? targetCamera.gameObject.AddComponent<PostProcessVolume>();
        
        if (volume.profile != null && volume.profile.name.Contains("Temp_"))
        {
            Destroy(volume.profile);
        }

        PostProcessProfile tempProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
        tempProfile.name = "Temp_" + fileNameToLoad;
        
        string[] lines = File.ReadAllLines(path);
        string section = "";

        foreach (string line in lines)
            {
                string row = line.Trim();
                if (string.IsNullOrEmpty(row) || row.StartsWith(";") || row.StartsWith("//")) continue;

                if (row.StartsWith("[") && row.EndsWith("]"))
                {
                    section = row.Substring(1, row.Length - 2);
                    continue;
                }

                string[] kv = row.Split('=');
                if (kv.Length != 2) continue;

                string key = kv[0].Trim();
                string rawValue = kv[1].Split(';')[0].Trim().ToLower();
                
                float val = 0;
                if (rawValue == "true") val = 1;
                else if (rawValue == "false") val = 0;
                else float.TryParse(rawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out val);

                ApplyAllSettings(tempProfile, section, key, val);
            }

        volume.enabled = true;
        volume.isGlobal = true;
        volume.priority = 99;
        volume.profile = tempProfile;
    }

    private void ApplyAllSettings(PostProcessProfile profile, string sec, string key, float val)
    {
        switch (sec)
        {
            case "ScreenSpaceReflections":
                var ssr = GetOrAdd<ScreenSpaceReflections>(profile);
                if (key == "Enabled") ssr.active = val > 0;
                if (key == "Preset") ssr.preset.Override((ScreenSpaceReflectionPreset)(int)val);
                if (key == "Thickness") ssr.thickness.Override(val);
                if (key == "DistanceFade") ssr.distanceFade.Override(val);
                if (key == "Vignette") ssr.vignette.Override(val);
                break;

            case "AmbientOcclusion":
                var ao = GetOrAdd<AmbientOcclusion>(profile);
                if (key == "Enabled") ao.active = val > 0;
                if (key == "Intensity") ao.intensity.Override(val);
                if (key == "Radius") ao.radius.Override(val);
                if (key == "Quality") ao.quality.Override((AmbientOcclusionQuality)(int)val);
                break;

            case "Bloom":
                var bloom = GetOrAdd<Bloom>(profile);
                if (key == "Enabled") bloom.active = val > 0;
                if (key == "Intensity") bloom.intensity.Override(val);
                if (key == "Threshold") bloom.threshold.Override(val);
                if (key == "SoftKnee") bloom.softKnee.Override(val);
                if (key == "Diffusion") bloom.diffusion.Override(val);
                if (key == "AnamorphicRatio") bloom.anamorphicRatio.Override(val);
                break;

            case "LensDistortion":
                var ld = GetOrAdd<LensDistortion>(profile);
                if (key == "Enabled") ld.active = val > 0;
                if (key == "Intensity") ld.intensity.Override(val);
                if (key == "XMultiplier") ld.intensityX.Override(val);
                if (key == "YMultiplier") ld.intensityY.Override(val);
                if (key == "Scale") ld.scale.Override(val);
                break;

            case "ColorGrading":
                var cg = GetOrAdd<ColorGrading>(profile);
                if (key == "Enabled") cg.active = val > 0;
                if (key == "Temperature") cg.temperature.Override(val);
                if (key == "Tint") cg.tint.Override(val);
                if (key == "PostExposure") cg.postExposure.Override(val);
                if (key == "Contrast") cg.contrast.Override(val);
                if (key == "Saturation") cg.saturation.Override(val);
                if (key.StartsWith("Shadows")) {
                    var v = cg.lift.value;
                    if (key.EndsWith("R")) v.x = val; else if (key.EndsWith("G")) v.y = val; else if (key.EndsWith("B")) v.z = val;
                    cg.lift.Override(v);
                }
                if (key.StartsWith("Highlights")) {
                    var v = cg.gain.value;
                    if (key.EndsWith("R")) v.x = val; else if (key.EndsWith("G")) v.y = val; else if (key.EndsWith("B")) v.z = val;
                    cg.gain.Override(v);
                }
                break;

            case "ChromaticAberration":
                var ca = GetOrAdd<ChromaticAberration>(profile);
                if (key == "Enabled") ca.active = val > 0;
                if (key == "Intensity") ca.intensity.Override(val);
                if (key == "FastMode") ca.fastMode.Override(val > 0);
                break;

            case "Grain":
                var grain = GetOrAdd<Grain>(profile);
                if (key == "Enabled") grain.active = val > 0;
                if (key == "Intensity") grain.intensity.Override(val);
                if (key == "Size") grain.size.Override(val);
                // 彻底修复报错：根据 PPv2 官方文档使用正确名称
                if (key == "LumContribution") grain.lumContrib.Override(val);
                break;

            case "Vignette":
                var vig = GetOrAdd<Vignette>(profile);
                if (key == "Enabled") vig.active = val > 0;
                if (key == "Intensity") vig.intensity.Override(val);
                if (key == "Smoothness") vig.smoothness.Override(val);
                if (key == "Roundness") vig.roundness.Override(val);
                break;

            case "MotionBlur":
                var mb = GetOrAdd<MotionBlur>(profile);
                if (key == "Enabled") mb.active = val > 0;
                if (key == "ShutterAngle") mb.shutterAngle.Override(val);
                if (key == "SampleCount") mb.sampleCount.Override((int)val);
                break;

            case "DepthOfField":
                var dof = GetOrAdd<DepthOfField>(profile);
                if (key == "Enabled") dof.active = val > 0;
                if (key == "FocusDistance") dof.focusDistance.Override(val);
                if (key == "Aperture") dof.aperture.Override(val);
                if (key == "FocalLength") dof.focalLength.Override(val);
                break;

            case "AutoExposure":
                var ae = GetOrAdd<AutoExposure>(profile);
                if (key == "Enabled") ae.active = val > 0;
                if (key == "MinLuminance") ae.minLuminance.Override(val);
                if (key == "MaxLuminance") ae.maxLuminance.Override(val);
                if (key == "KeyValue") ae.keyValue.Override(val);
                break;
        }
    }

    private T GetOrAdd<T>(PostProcessProfile profile) where T : PostProcessEffectSettings
    {
        if (!profile.HasSettings<T>()) profile.AddSettings<T>();
        var s = profile.GetSetting<T>();
        s.active = true;
        s.enabled.Override(true);
        return s;
    }
}