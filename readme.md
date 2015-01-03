#OculusRiftDK2ExtendPreview

OculusRift DK2を使ったコンテンツをUnityで作っているとき、「拡張ディスプレイ扱いのDK2側に、今のシーンを全画面表示したい」 ことが良くあります。 
そんな感じの事を行うUnityのエディタ拡張スクリプトを先人が書いていた(DK1用)ので、それをアップデートしました。

#Setup

1. OculusConfiguratiom UtilityでDK2をExtendModeにします。

DK2をメイン画面の右側に配置します

2. Unity上でAssets→ImportPackage→CustumPackage→ "DK2ExtendPreview.unitypackage"を既存プロジェクトに追加。
 

#Usage

1. UnityEditor上でWindow → Rift VR Game Modeをクリック。GameViewが消滅して、RiftModeというウィンドウが出来ていることを確認 
2. RiftModeウィンドウを出したまま、普通にUnity上で再生ボタンを押す
3. シーン再生中にEscを押すと通常ウィンドウに復帰

#Environment

- Windows 7-8.1
- OculusRiftDK2:ExtendMode
- Unity:4.5.2- 4.6.1
- OVRSDK 0.4.2-0.4.4
- OculusRuntime 0.4.2-0.4.4

#Document

解説及び詳細説明
@izm
http://izm-11.hatenablog.com/entry/2014/08/13/183846

他の有料アセットとの動作比較及びUnity4.6での動作検証
@cubic9com
http://cubic9.com/Devel/OculusRift/Extend%A5%E2%A1%BC%A5%C9%A4%AB%A4%C4Unity%A4%CE%A5%D7%A5%EC%A5%A4%A5%E2%A1%BC%A5%C9%BB%FE%A4%CBRift%C2%A6%A4%CB%C9%BD%BC%A8%A4%B9%A4%EB/