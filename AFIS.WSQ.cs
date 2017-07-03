/*****************************************************/
/* AFIS.WSQ                                          */
/* 01-Junio-2008                                     */
/*****************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security;

namespace AFIS.WSQ
{

  /// <summary>
  /// Clase para la manipulación de imágenes en formato WSQ
  /// </summary>
  [SuppressUnmanagedCodeSecurity]
  public class WSQ
  {
    /// <summary>
    ///	Convierte de Bitmap a formato WSQ
    /// </summary>
    /// <param name="bmp">Imagen que se desea convertir</param>
    /// <param name="context">Contexto del WSQ</param>
    /// <returns>Retorna un arreglo de bytes con la imagen en formato WSQ</returns>
    public static byte[] BitmapToWSQ(Bitmap bmp, IntPtr context)
    {
      int size = 0;
      var bufferOut = new byte[bmp.Width * bmp.Height];
      var bmpRaw = BitmapToRaw(bmp);
      int error = RawImageToWSQ(bmpRaw,
                                bmp.Width,
                                bmp.Height,
                                ref size,
                                bufferOut, context);
      if (error != 0) throw new Exception("Error en compresión: " + error);
      Array.Resize(ref bufferOut, size);
      return bufferOut;
    }

    /// <summary>
    ///	Convierte de Bitmap a formato WSQ
    /// </summary>
    /// <param name="bmp">Imagen que se desea convertir</param>
    /// <returns>Retorna un arreglo de bytes con la imagen en formato WSQ</returns>
    public static byte[] BitmapToWSQ(Bitmap bmp)
    {
      var context = WSQCreateContext();
      var result = BitmapToWSQ(bmp, context);
      WSQFreeContext(context);
      return result;
    }

    /// <summary>
    ///	 Convierte un formato WSQ a formato Bitmap
    /// </summary>
    /// <param name="wsq">Arreglo que contiene la imagen en formato WSQ</param>
    /// <param name="context">Contexto del WSQ</param>
    /// <returns>Bitmap</returns>
    public static Bitmap WSQToBitmap(byte[] wsq, IntPtr context)
    {
      int w = 0, h = 0, d = 0, p = 0;
      Bitmap bmpResult = null;
      int error = WSQGetDimensions(wsq, wsq.Length, ref w, ref h, context);
      if (error != 0) throw new Exception("Error en formato de archivo: " + error);
      var bufferOut = new byte[w * h];
      error = WSQToRawImage(wsq, wsq.Length, ref w, ref h, ref d, ref p, bufferOut, context);
      if (error == 0)
      {
        bmpResult = RawToBitmap(bufferOut, w, h);
      }
      if (error != 0) throw new Exception("Error descomprimiendo: " + error);
      return bmpResult;
    }

    /// <summary>
    ///	 Convierte un formato WSQ a formato Bitmap
    /// </summary>
    /// <param name="wsq">Arreglo que contiene la imagen en formato WSQ</param>
    /// <returns>Bitmap</returns>
    public static Bitmap WSQToBitmap(byte[] wsq)
    {
      var context = WSQCreateContext();
      var result = WSQToBitmap(wsq, context);
      WSQFreeContext(context);
      return result;
    }


    /// <summary>
    /// Convierte un Bitmap a formato RAW
    /// </summary>
    /// <param name="bmp">Bitmap</param>
    /// <returns>Arreglo de byte, imagen en formato raw</returns>
    public static byte[] BitmapToRaw(Bitmap bmp)
    {
      BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                        ImageLockMode.ReadWrite, bmp.PixelFormat);

      var bt = new byte[((bmp.Width) * (bmp.Height))];
      FastBitmapToRaw(bmp.PixelFormat, bmp.Width, bmp.Height, bmData.Stride, bmData.Scan0, bt);
      bmp.UnlockBits(bmData);
      return bt;
    }

      /// <summary>
      /// Convierte una imagen en formato RAW a Bitmap
      /// </summary>
      /// <param name="raw">Imagen en formato RAW</param>
      /// <param name="width">Ancho de la imagen</param>
      /// <param name="height">Alto de la imagen</param>
      /// <returns>Bitmap en escala de grises</returns>
      public static Bitmap RawToBitmap(byte[] raw, int width, int height)
    {
      var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
      var cp = bmp.Palette;
      for (var i = 0; i < 256; i++) cp.Entries[i] = Color.FromArgb(i, i, i);
      bmp.Palette = cp;
      BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                        ImageLockMode.ReadWrite, bmp.PixelFormat);
      FastRawToBitmap(width, height, bmData.Stride, bmData.Scan0, raw);
      bmp.UnlockBits(bmData);
      return bmp;
    }


    private const string WSQDLL = "AFIS.WSQ.K.dll";

    [DllImport(WSQDLL)]
    private static extern int RawImageToWSQ(byte[] raw, int w, int h, ref int size, byte[] bufferOut, IntPtr context);

    [DllImport(WSQDLL)]
    private static extern int WSQToRawImage(byte[] wsq, int size, ref int width, ref int height, ref int depth, ref int ppi, byte[] raw, IntPtr context);

    [DllImport(WSQDLL)]
    private static extern int WSQGetDimensions(byte[] wsq, int size, ref int width, ref int height, IntPtr context);

    [DllImport(WSQDLL)]
    private static extern IntPtr WSQCreateContext();

    [DllImport(WSQDLL)]
    private static extern int WSQFreeContext(IntPtr context);

    [DllImport(WSQDLL)]
    private static extern void FastBitmapToRaw(PixelFormat pixelformat, int width, int height, int stride, IntPtr scan,
                                               byte[] barray);
    [DllImport(WSQDLL)]
    private static extern void FastRawToBitmap(int width, int height, int stride, IntPtr scan, byte[] barray);

  }
}