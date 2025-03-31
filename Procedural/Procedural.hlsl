#include "Assets/Procedural/Procedural_Declarations.hlsl"
#include "Assets/Procedural/Procedural_Input.hlsl"
#include "Assets/Procedural/ProceduralUTils.hlsl"


Varyings Procedural_Vertex(Attributes input,uint id_:SV_InstanceID)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
  // Set_UnityPerMaterialBufferData();
    
    VertexPositionInputs vertexInput = Get_VertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = Get_VertexNormalInputs(input.normalOS, input.tangentOS);
    half3 vertexLight = Vertex_Lighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = 0;
    MainUV(input.texcoord,output.uv);
    output.normalWS = normalInput.normalWS;

    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR) || defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    real sign = input.tangentOS.w * GetOddNegativeScale();
    half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
    #endif
    #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    output.tangentWS = tangentWS;
    #endif
    
    #if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
    half3 viewDirTS = GetViewDirectionTangentSpace(tangentWS, output.normalWS, viewDirWS);
    output.viewDirTS = viewDirTS;
    #endif

    OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
    output.fogFactor = fogFactor;

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    output.positionWS = vertexInput.positionWS;
    #endif

    output.positionCS = vertexInput.positionCS;
    
    return output;
}

half4 Procedural_Fragment(Varyings input):SV_Target0
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);   
    
    FragmentData fragdata=GetFragmentData(input);
    SurfaceData surfaceData;
    Initialize_StandardLitSurfaceData(input.uv, surfaceData,fragdata);

    InputData inputData;
    Initialize_InputData(input, surfaceData.normalTS, inputData);
    half4 color = UniversalFragmentPBR(inputData, surfaceData);
    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    color.a =OutputAlpha(color.a, IsSurfaceTypeTransparent(_Surface));
    
    return color;
}