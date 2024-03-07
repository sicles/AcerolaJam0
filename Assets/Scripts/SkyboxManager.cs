using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    [SerializeField] private float skyboxRotationSpeed = 5f;
    [SerializeField] private float skyboxRotation;
    private static readonly int Rotation = Shader.PropertyToID("_Rotation");

    private void Update()
    {
        RotateSkybox();
    }

    private void RotateSkybox()
    {
        if (skyboxRotation > 360)
            skyboxRotation = 0;
        skyboxRotation += skyboxRotationSpeed * Time.deltaTime;
        RenderSettings.skybox.SetFloat(Rotation, skyboxRotation);
    }
    
    
}
