# Mouse2Touch
Mouse2Touch 能在任何普通的滑鼠上新增觸控功能，<br>
一般的滑鼠只有一個垂直捲動的滾輪，在使用瀏覽器、vs code、Excel 之類的程式時，<br>
如果要水平捲動或是微調捲動，就會非常麻煩<br>
只要使用 Mouse2Touch 就能讓滑鼠像在觸控螢幕上一樣，直接拖曳整個頁面
<br>



- 運行環境：win8、win10
- 運行需求：net framework 4.5
- 提供語言：English、日文、中文

### 下載
https://github.com/hbl917070/Mouse2Touch/releases/


### 程式截圖
<img src="https://i.imgur.com/V1blI8T.png" width="400">


### 實際使用
<img src="https://i.imgur.com/uEOLveJ.gif" width="400">


### 補充說明
因為在模擬觸控的過程中，原本的滑鼠指標也在移動，這會使得少數一些原本支援觸控的程式，無法以本程式的模擬觸控進行拖曳，像是「office 2016 OneNote」就有這個問題

Mouse2Touch 不能在用系統管理員身份執行的程式上面工作，把 Mouse2Touch 也使用系統管理員身份執行，就能解決這個問題



# Project
- 開發環境：visual studio 2019
- 專案類型：C# winform
- UI ：webbrowser(HTML、CSS、JavaScript)

模擬觸控 、 攔截滑鼠訊號 、 偵測目前焦點視窗，都是以 Win32 API 實現
