using NoteEditor.Model;
using NoteEditor.Utility;
using UniRx;
using UnityEngine;

namespace NoteEditor.GLDrawing
{
    public class NoteRenderer : MonoBehaviour
    {
        void LateUpdate()
        {
            if (Audio.Source.clip == null)
                return;

            foreach (var noteObj in EditData.Notes.Values)
            {
                var canvasPosOfNote = ConvertUtils.NoteToCanvasPosition(noteObj.note.position);
                var min = ConvertUtils.ScreenToCanvasPosition(Vector3.zero).x;
                var max = ConvertUtils.ScreenToCanvasPosition(Vector3.right * Screen.width).x * 1.1f;

                if (min <= canvasPosOfNote.x && canvasPosOfNote.x <= max)
                {
                    // ノーツ描写
                    noteObj.LateUpdateObservable.OnNext(Unit.Default);
                    var screenPos = ConvertUtils.CanvasToScreenPosition(canvasPosOfNote);
                    if (noteObj.note.type == Notes.NoteTypes.TopFlick)
                    {
                        // **************************************************
                        // 上フリック
                        // **************************************************
                        var vertexTriangleX = 0;
                        var vertexTriangleY = 14 / NoteCanvas.ScaleFactor.Value;
                        var centerX = vertexTriangleX / 2;
                        var centerY = vertexTriangleY / 2;
                        GLQuadDrawer.Draw(new Geometry(new[] {
                            // 頂点
                            new Vector3(screenPos.x + vertexTriangleX - centerX, screenPos.y + vertexTriangleY - centerY, 0),
                            // →
                            new Vector3(centerY == 0 ? screenPos.x - centerX : screenPos.x - centerY - centerX, centerX == 0 ? screenPos.y - centerY : screenPos.y - centerX - centerY, 0),
                            // 中点
                            new Vector3(screenPos.x, screenPos.y, 0),
                            // ←
                            new Vector3(centerY == 0 ? screenPos.x - centerX : screenPos.x + centerY - centerX, centerX == 0 ? screenPos.y - centerY : screenPos.y + centerX - centerY, 0)
                        }, noteObj.NoteColor));
                    }
                    else if (noteObj.note.type == Notes.NoteTypes.RightFlick)
                    {
                        // **************************************************
                        // 右フリック
                        // **************************************************
                        var vertexTriangleX = 14 / NoteCanvas.ScaleFactor.Value;
                        var vertexTriangleY = 0;
                        var centerX = vertexTriangleX / 2;
                        var centerY = vertexTriangleY / 2;
                        GLQuadDrawer.Draw(new Geometry(new[] {
                            // 頂点
                            new Vector3(screenPos.x + vertexTriangleX - centerX, screenPos.y + vertexTriangleY - centerY, 0),
                            // →
                            new Vector3(centerY == 0 ? screenPos.x - centerX : screenPos.x - centerY - centerX, centerX == 0 ? screenPos.y - centerY : screenPos.y - centerX - centerY, 0),
                            // 中点
                            new Vector3(screenPos.x, screenPos.y, 0),
                            // ←
                            new Vector3(centerY == 0 ? screenPos.x - centerX : screenPos.x + centerY - centerX, centerX == 0 ? screenPos.y - centerY : screenPos.y + centerX - centerY, 0)
                        }, noteObj.NoteColor));
                    }
                    else if (noteObj.note.type == Notes.NoteTypes.BottomFlick)
                    {
                        // **************************************************
                        // 下フリック
                        // **************************************************
                        var vertexTriangleX = 0;
                        var vertexTriangleY = -14 / NoteCanvas.ScaleFactor.Value;
                        var centerX = vertexTriangleX / 2;
                        var centerY = vertexTriangleY / 2;
                        GLQuadDrawer.Draw(new Geometry(new[] {
                            // 頂点
                            new Vector3(screenPos.x + vertexTriangleX - centerX, screenPos.y + vertexTriangleY - centerY, 0),
                            // →
                            new Vector3(centerY == 0 ? screenPos.x - centerX : screenPos.x - centerY - centerX, centerX == 0 ? screenPos.y - centerY : screenPos.y - centerX - centerY, 0),
                            // 中点
                            new Vector3(screenPos.x, screenPos.y, 0),
                            // ←
                            new Vector3(centerY == 0 ? screenPos.x - centerX : screenPos.x + centerY - centerX, centerX == 0 ? screenPos.y - centerY : screenPos.y + centerX - centerY, 0)
                        }, noteObj.NoteColor));
                    }
                    else if (noteObj.note.type == Notes.NoteTypes.LeftFlick)
                    {
                        // **************************************************
                        // 左フリック
                        // **************************************************
                        var vertexTriangleX = -14 / NoteCanvas.ScaleFactor.Value;
                        var vertexTriangleY = 0;
                        var centerX = vertexTriangleX / 2;
                        var centerY = vertexTriangleY / 2;
                        GLQuadDrawer.Draw(new Geometry(new[] {
                            // 頂点
                            new Vector3(screenPos.x + vertexTriangleX - centerX, screenPos.y + vertexTriangleY - centerY, 0),
                            // →
                            new Vector3(centerY == 0 ? screenPos.x - centerX : screenPos.x - centerY - centerX, centerX == 0 ? screenPos.y - centerY : screenPos.y - centerX - centerY, 0),
                            // 中点
                            new Vector3(screenPos.x, screenPos.y, 0),
                            // ←
                            new Vector3(centerY == 0 ? screenPos.x - centerX : screenPos.x + centerY - centerX, centerX == 0 ? screenPos.y - centerY : screenPos.y + centerX - centerY, 0)
                        }, noteObj.NoteColor));
                    }
                    else if (noteObj.note.type == Notes.NoteTypes.TopRightFlick)
                    {
                        // **************************************************
                        // 右上フリック
                        // **************************************************
                        // 頂点x座標
                        var vertexTriangleX = 14 / NoteCanvas.ScaleFactor.Value / Mathf.Sqrt(2);
                        // 頂点y座標
                        var vertexTriangleY = 14 / NoteCanvas.ScaleFactor.Value / Mathf.Sqrt(2);
                        // 中点座標への移動距離
                        var centerX = vertexTriangleX / 2;
                        var centerY = vertexTriangleY / 2;
                        GLQuadDrawer.Draw(new Geometry(new[] {
                            // 頂点
                            new Vector3(screenPos.x + vertexTriangleX - centerX, screenPos.y + vertexTriangleY - centerY, 0),
                            // →
                            new Vector3(centerX - centerY == 0 ? screenPos.x : screenPos.x - centerX - centerX, centerX - centerY == 0 ? screenPos.y - centerY - centerY : screenPos.y, 0),
                            // 中点
                            new Vector3(screenPos.x, screenPos.y, 0),
                            // ←
                            new Vector3(centerX - centerY == 0 ? screenPos.x - centerX - centerX : screenPos.x, centerX - centerY == 0 ? screenPos.y : screenPos.y - centerY - centerY, 0)
                        }, noteObj.NoteColor));
                    }
                    else if (noteObj.note.type == Notes.NoteTypes.BottomRightFlick)
                    {
                        // **************************************************
                        // 右下フリック
                        // **************************************************
                        // 頂点x座標
                        var vertexTriangleX = 14 / NoteCanvas.ScaleFactor.Value / Mathf.Sqrt(2);
                        // 頂点y座標
                        var vertexTriangleY = -14 / NoteCanvas.ScaleFactor.Value / Mathf.Sqrt(2);
                        // 中点座標への移動距離
                        var centerX = vertexTriangleX / 2;
                        var centerY = vertexTriangleY / 2;
                        GLQuadDrawer.Draw(new Geometry(new[] {
                            // 頂点
                            new Vector3(screenPos.x + vertexTriangleX - centerX, screenPos.y + vertexTriangleY - centerY, 0),
                            // →
                            new Vector3(centerX - centerY == 0 ? screenPos.x : screenPos.x - centerX - centerX, centerX - centerY == 0 ? screenPos.y - centerY - centerY : screenPos.y, 0),
                            // 中点
                            new Vector3(screenPos.x, screenPos.y, 0),
                            // ←
                            new Vector3(centerX - centerY == 0 ? screenPos.x - centerX - centerX : screenPos.x, centerX - centerY == 0 ? screenPos.y : screenPos.y - centerY - centerY, 0)
                        }, noteObj.NoteColor));
                    }
                    else if (noteObj.note.type == Notes.NoteTypes.BottomLeftFlick)
                    {
                        // **************************************************
                        // 左下フリック
                        // **************************************************
                        // 頂点x座標
                        var vertexTriangleX = -14 / NoteCanvas.ScaleFactor.Value / Mathf.Sqrt(2);
                        // 頂点y座標
                        var vertexTriangleY = -14 / NoteCanvas.ScaleFactor.Value / Mathf.Sqrt(2);
                        // 中点座標への移動距離
                        var centerX = vertexTriangleX / 2;
                        var centerY = vertexTriangleY / 2;
                        GLQuadDrawer.Draw(new Geometry(new[] {
                            // 頂点
                            new Vector3(screenPos.x + vertexTriangleX - centerX, screenPos.y + vertexTriangleY - centerY, 0),
                            // →
                            new Vector3(centerX - centerY == 0 ? screenPos.x : screenPos.x - centerX - centerX, centerX - centerY == 0 ? screenPos.y - centerY - centerY : screenPos.y, 0),
                            // 中点
                            new Vector3(screenPos.x, screenPos.y, 0),
                            // ←
                            new Vector3(centerX - centerY == 0 ? screenPos.x - centerX - centerX : screenPos.x, centerX - centerY == 0 ? screenPos.y : screenPos.y - centerY - centerY, 0)
                        }, noteObj.NoteColor));
                    }
                    else if (noteObj.note.type == Notes.NoteTypes.TopLeftFlick)
                    {
                        // **************************************************
                        // 左上フリック
                        // **************************************************
                        // 頂点x座標
                        var vertexTriangleX = -14 / NoteCanvas.ScaleFactor.Value / Mathf.Sqrt(2);
                        // 頂点y座標
                        var vertexTriangleY = 14 / NoteCanvas.ScaleFactor.Value / Mathf.Sqrt(2);
                        // 中点座標への移動距離
                        var centerX = vertexTriangleX / 2;
                        var centerY = vertexTriangleY / 2;
                        GLQuadDrawer.Draw(new Geometry(new[] {
                            // 頂点
                            new Vector3(screenPos.x + vertexTriangleX - centerX, screenPos.y + vertexTriangleY - centerY, 0),
                            // →
                            new Vector3(centerX - centerY == 0 ? screenPos.x : screenPos.x - centerX - centerX, centerX - centerY == 0 ? screenPos.y - centerY - centerY : screenPos.y, 0),
                            // 中点
                            new Vector3(screenPos.x, screenPos.y, 0),
                            // ←
                            new Vector3(centerX - centerY == 0 ? screenPos.x - centerX - centerX : screenPos.x, centerX - centerY == 0 ? screenPos.y : screenPos.y - centerY - centerY, 0)
                        }, noteObj.NoteColor));
                    }
                    else
                    {
                        // **************************************************
                        // フリック以外
                        // **************************************************
                        // 実際の向きでいう幅
                        var drawWidth = 9 / NoteCanvas.ScaleFactor.Value;
                        // 実際の向きでいう高さ
                        var drawHeight = 5 / NoteCanvas.ScaleFactor.Value;
                        //Debug.Log(noteObj.note.next);
                        GLQuadDrawer.Draw(new Geometry(new[] {
                            // ┐
                            new Vector3(screenPos.x + drawHeight, screenPos.y + drawWidth, 0),
                            // ┘
                            new Vector3(screenPos.x + drawHeight, screenPos.y - drawWidth, 0),
                            // └
                            new Vector3(screenPos.x - drawHeight, screenPos.y - drawWidth, 0),
                            // ┌
                            new Vector3(screenPos.x - drawHeight, screenPos.y + drawWidth, 0)
                        }, noteObj.NoteColor));
                    }

                    if (noteObj.note.type == Notes.NoteTypes.StraightLineLong && EditData.Notes.ContainsKey(noteObj.note.prev))
                    {
                        EditData.Notes[noteObj.note.prev].LateUpdateObservable.OnNext(Unit.Default);
                    }
                }
            }
        }
    }
}
