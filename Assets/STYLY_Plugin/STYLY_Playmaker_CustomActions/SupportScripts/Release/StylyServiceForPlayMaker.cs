﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// handler class to handle requests from PlayMaker
    /// </summary>
    public interface IStylyServiceForPlayMakerImpl
    {
        void ChangeStylyScene(string sceneId, Action<Exception> onFinished);

        void OpenURL(string url, bool directOpen, Action<Exception> onFinished);

        void ChangeShader(string srcShaderName, string dstShaderName, Action<Exception> onFinished);

        void ChangeShadowDistance(float shadowDistance, GameObject ownerObj);

        void ResolveVRMScale(Transform vrmRootNode, float scale);

        void SetImageEffect(Material material, GameObject ownerObj, Action<Exception> onFinished);

        void SetProjectionTextureMapping(Material material, GameObject ownerObj);

        void GetARCameraImage(Action<Texture> onFinished);
        
        void StampRallyGet(string stampRallyId, int stampNumber, out bool result, Action<Exception> onFinished);
        void StampRallySet(string stampRallyId, int stampNumber, bool check, Action<Exception> onFinished);
        void StampRallyReset(string stampRallyId, int stampCount, Action<Exception> onFinished);
        void StampRallyIsComplete(string stampRallyId, int stampCount, out bool result, Action<Exception> onFinished);
    }

    /// <summary>
    /// A singleton hub class to use STYLY features from
    /// PlayMaker custom actions.
    /// </summary>
    public class StylyServiceForPlayMaker
    {
        private static StylyServiceForPlayMaker instance;

        public static StylyServiceForPlayMaker Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                instance = new StylyServiceForPlayMaker();
                return instance;
            }
        }

        private IStylyServiceForPlayMakerImpl impl;

        public void SetImpl(IStylyServiceForPlayMakerImpl impl)
        {
            this.impl = impl;
        }

        /// <summary>
        /// a service to change styly scene.
        /// </summary>
        /// <param name="sceneId">Scene Id (GUID format like: 1234abc-1234-abcd-ef12-012345abcdef)</param>
        public void ChangeStylyScene(string sceneId, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.ChangeStylyScene(sceneId, onFinished);
            }
            else
            {
                Debug.Log("ChangeStylyScene called with sceneId: <" + sceneId + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// a service to open web page.
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="directOpen">アプリ内で開く実装だった場合でも、直接標準ブラウザで開くことを指示します。</param>
        public void OpenURL(string url, bool directOpen, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.OpenURL(url, directOpen, onFinished);
            }
            else
            {
                Debug.Log("OpenURL called with url: <" + url + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// STYLYシーン内の全てのアセットに対して、シェーダーの入れ替えを行う
        /// </summary>
        /// <param name="srcShaderName">対象シェーダー名</param>
        /// <param name="dstShaderName">入れ替えシェーダー名</param>
        public void ChangeShader(string srcShaderName, string dstShaderName, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.ChangeShader(srcShaderName, dstShaderName, onFinished);
            }
            else
            {
                Debug.Log("ChangeShader called with srcShaderName: <" + srcShaderName + ">" + "dstShaderName: <" + dstShaderName + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// QualitySettings.shadowDistanceを変更する。
        /// 変更した値は、指定オブジェクトが破棄されたタイミングで元の値に戻される。
        /// </summary>
        /// <param name="shadowDistance">shadowDisctance</param>
        /// <param name="ownerObj">このオブジェクトが破棄されるタイミングで元のshadowDistance値に戻される。</param>
        public void ChangeShadowDistance(float shadowDistance, GameObject ownerObj, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.ChangeShadowDistance(shadowDistance, ownerObj);
            }
            else
            {
                Debug.Log("ChangeShadowDistance called With shaderDistance<" + shadowDistance + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// 単位ScaleではないVRMモデルの調整を行う。
        /// </summary>
        /// <param name="vrmRootNode">VRMのルートノード</param>
        /// <param name="scale">スケール値</param>
        public void ResolveVRMScale(Transform vrmRootNode, float scale)
        {
            if (impl != null)
            {
                impl.ResolveVRMScale(vrmRootNode, scale);
            }
            else
            {
                Debug.Log("ResolveVRMScale called");
            }
        }

        /// <summary>
        /// カメラにイメージエフェクトをセットする。
        /// </summary>
        /// <param name="material"></param>
        /// <param name="ownerObj"></param>
        /// <param name="onFinished"></param>
        public void SetImageEffect(Material material, GameObject ownerObj, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.SetImageEffect(material, ownerObj, onFinished);
            }
            else
            {
                Debug.Log("SetImageEffect called With material<" + material + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// カメラの映像を使って射影テクスチャマッピングをセットする。
        /// </summary>
        /// <param name="material"></param>
        /// <param name="ownerObj"></param>
        /// <param name="onFinished"></param>
        public void SetProjectionTextureMapping(Material material, GameObject ownerObj, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.SetProjectionTextureMapping(material, ownerObj);
            }
            else
            {
                Debug.Log("SetProjectionTextureMapping called With material<" + material + ">");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// ARカメラの映像をテクスチャとして取得する
        /// </summary>
        /// <param name="onFinished"></param>
        public void GetARCameraImage(Action<Texture> onFinished)
        {
            if (impl != null)
            {
                impl.GetARCameraImage(onFinished);
            }
            else
            {
                onFinished(null);
            }
        }
        
        /// <summary>
        /// 指定スタンプラリーの指定番号のチェック状況を返す。
        /// </summary>
        /// <param name="stampRallyId">スタンプラリーID</param>
        /// <param name="number">スタンプ番号</param>
        /// <param name="result">チェックされていればtrue</param>
        /// <param name="onFinished"></param>
        public void StampRallyGet(string stampRallyId, int number, out bool result, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.StampRallyGet(stampRallyId, number, out result, onFinished);
            }
            else
            {
                Debug.Log($"StampRallyGet called with stampRallyId<{stampRallyId}>");
                result = false;
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// 指定スタンプラリーの指定番号のチェック状況をセットする。
        /// </summary>
        /// <param name="stampRallyId">スタンプラリーID</param>
        /// <param name="number">スタンプ番号</param>
        /// <param name="onFinished"></param>
        public void StampRallySet(string stampRallyId, int number, bool check, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.StampRallySet(stampRallyId, number, check, onFinished);
            }
            else
            {
                Debug.Log($"StampRallyGet called with stampRallyId<{stampRallyId}>");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// 指定スタンプラリーのすべての番号を未チェック状態に戻す。
        /// </summary>
        /// <param name="stampRallyId">スタンプラリーID</param>
        /// <param name="stampCount">スタンプの総数</param>
        /// <param name="onFinished"></param>
        public void StampRallyReset(string stampRallyId, int stampCount, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.StampRallyReset(stampRallyId, stampCount, onFinished);
            }
            else
            {
                Debug.Log($"StampRallyGet called with stampRallyId<{stampRallyId}>");
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }

        /// <summary>
        /// 指定スタンプラリーの全ての番号がチェックされているかどうかを返す。
        /// </summary>
        /// <param name="stampRallyId">スタンプラリーID</param>
        /// <param name="stampCount">スタンプの総数</param>
        /// <param name="result">チェックされていればtrue</param>
        /// <param name="onFinished"></param>
        public void StampRallyIsComplete(string stampRallyId, int stampCount, out bool result, Action<Exception> onFinished)
        {
            if (impl != null)
            {
                impl.StampRallyIsComplete(stampRallyId, stampCount, out result, onFinished);
            }
            else
            {
                Debug.Log($"StampRallyGet called with stampRallyId<{stampRallyId}>");
                result = false;
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }
    }
}
