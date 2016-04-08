using UnityEngine;
using System.Runtime.InteropServices;

/// <summary>
/// 粒子の構造体
/// </summary>
struct Particle
{
    /// <summary>
    /// 座標
    /// </summary>
    public Vector3 pos;

    /// <summary>
    /// 速度
    /// </summary>
    public Vector3 accel;

    /// <summary>
    /// 色
    /// </summary>
    public Color color;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Particle(Vector3 pos, Vector3 accel, Color color)
    {
        this.pos = pos;
        this.accel = accel;
        this.color = color;
    }
}

/// <summary>
/// 沢山の粒子を管理するクラス
/// </summary>
public class LightEffects : MonoBehaviour
{

    /// <summary>
    /// レンダリングするシェーダー
    /// </summary>
    public Shader ParticleShader;

    /// <summary>
    /// テクスチャ
    /// </summary>
    public Texture ParticleTexture;

    /// <summary>
    /// 更新を行うコンピュートシェーダー
    /// </summary>
    public ComputeShader ParticleComputeShader;

    /// <summary>
    /// マテリアル
    /// </summary>
    Material ParticleMaterial;

    /// <summary>
    /// コンピュートバッファ
    /// </summary>
    ComputeBuffer ParticleBuffer;

    [SerializeField]
    int maxparticles=10000;


    /// <summary>
    /// 破棄
    /// </summary>
    void OnDisable()
    {
        // コンピュートバッファは明示的に破棄
        ParticleBuffer.Release();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Start()
    {
        ParticleMaterial = new Material(ParticleShader);
        InitializeComputeBuffer();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        ParticleComputeShader.SetBuffer(0, "Bullets", ParticleBuffer);
        ParticleComputeShader.SetFloat("DeltaTime", Time.deltaTime);
        ParticleComputeShader.SetFloat("time",Time.time);
        ParticleComputeShader.Dispatch(0, ParticleBuffer.count / 8 + 1, 1, 1);
    }

    /// <summary>
    /// コンピュートバッファの初期化
    /// </summary>
    void InitializeComputeBuffer()
    {
        ParticleBuffer = new ComputeBuffer(maxparticles, Marshal.SizeOf(typeof(Particle)));

        // 配列に初期値を代入する
        Particle[] bullets = new Particle[ParticleBuffer.count];
        for (int i = 0; i < ParticleBuffer.count; i++)
        {
            float ran = Random.Range(0.0f, 1.0f);
            bullets[i] =
                new Particle(
                    new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(0f, 10.0f), Random.Range(-10.0f, 10.0f))*7.0f,
                    new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 0.2f,
                    new Color(1, 1, 1-ran*ran));
        }

        // バッファに適応
        ParticleBuffer.SetData(bullets);
    }

    /// <summary>
    /// レンダリング
    /// </summary>
    void OnRenderObject()
    {

        // テクスチャ、バッファをマテリアルに設定
        ParticleMaterial.SetTexture("_MainTex", ParticleTexture);
        ParticleMaterial.SetBuffer("Bullets", ParticleBuffer);

        // レンダリングを開始
        ParticleMaterial.SetPass(0);

        //オブジェクトをレンダリング
        Graphics.DrawProcedural(MeshTopology.Points, ParticleBuffer.count);
    }

}