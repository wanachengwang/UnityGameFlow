using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Zenject;
using Utils;

public class GameVerMgr : IInitializable {
    //TODO: Integration Integration
    //  Check local files' integretion, if fail, redownload
    //  Get local version digest and post to version server
    //  Get result from version server

    //Step1: Request for FileUpdateManifest
    //          count<0: mean cannot update in game, need reinstall
    //          count=0: mean no update available, the latest version already
    //          fileUrlToDownload, filePathNameToStore, VerificatinCode(CRC/Length/TBD)
    //Step2: Request for each file and save to local.

    public enum ErrorType {
        None,
        FailedGetManifest,
        WrongFormatManifest,
        WrongVersionToUpdate,
        FailedGetAFile,
        FailedWriteAFile,
    }

    string _versionCheckUrl;    // url + current version token, ex. http://xxxx.xxxx.com/module/controller/list?token=6918b5e0567c7d47314c10ed413e3f95

    string _majorVersion;       // Main version
    string _minorVersion;       // Patch version

    [Inject]
    MonoBehaviour _coroutineRunner;

    [Inject]
    Func<ErrorType, string, IEnumerator> _onError;

    void IInitializable.Initialize() {
        _coroutineRunner.StartCoroutine(UpdateVersion());
    }
    IEnumerator UpdateVersion() {
        WWW www0;
        bool bError = false;
        CoroutineWithResult coroutine = null;
        System.Object retry = null;
        do {
            www0 = new WWW(_versionCheckUrl);
            yield return www0;

            bError = www0.error != null;
            if (bError) {
                coroutine = new CoroutineWithResult(_coroutineRunner, _onError(ErrorType.FailedGetManifest, www0.error));
                yield return coroutine.Coroutine;
                retry = coroutine.Result;
            } else if(!VerifyManifest(www0.text)) {
                bError = true;
                coroutine = new CoroutineWithResult(_coroutineRunner, _onError(ErrorType.WrongFormatManifest, www0.text));
                yield return coroutine.Coroutine;
                retry = coroutine.Result;
            }
        } while (retry != null);
        if (bError) {
            www0.Dispose();
            yield break;
        }

        using (StringReader sr = new StringReader(www0.text)) {
            WWW www1;
            int cnt = int.Parse(sr.ReadLine());
            for(int i = 0; i < cnt; i++) {
                string filePair = sr.ReadLine();
                string[] fileUrlLen = filePair.Split(',');
                do {
                    www1 = new WWW(fileUrlLen[0]);
                    yield return www1;

                    bError = www1.error != null;
                    if (bError) {
                        coroutine = new CoroutineWithResult(_coroutineRunner, _onError(ErrorType.FailedGetAFile, www1.error));
                        yield return coroutine.Coroutine;
                        retry = coroutine.Result;
                    } else if(!WriteAFile(www0.bytes, fileUrlLen)) {
                        bError = true;
                        coroutine = new CoroutineWithResult(_coroutineRunner, _onError(ErrorType.FailedWriteAFile, filePair));
                        yield return coroutine.Coroutine;
                        retry = coroutine.Result;
                    }
                    www1.Dispose();
                } while (retry != null);
                if (bError) {
                    www0.Dispose();
                    yield break;
                }
            }        
        }        
    }

    bool VerifyManifest(string manifest) {

        return true;
    }

    bool WriteAFile(byte[] dat, string[] fileUrlLen) {
        try {
            if (dat.Length == int.Parse(fileUrlLen[2])) {
                File.WriteAllBytes(fileUrlLen[1], dat);
                return true;
            }
        } catch (Exception e) {
            Debug.LogError("Failed to write file:" + e.ToString());
        }        
        return false;
    }

}
