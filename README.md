# AFIS.WSQ

Ensamblado .NET para la compresión de las impresiones dactilares en formato WSQ según NBIS (<a href="https://www.nist.gov/services-resources/software/nist-biometric-image-software-nbis">NIST Biometric Image Software</a>). Hace link con AFIS.WSQ.K

WSQ es un algoritmo para la compresión de impresiones dactilares en escala de colores de gris (FBI). Es una librería de código abierto la cual toma una impresión dactilar y la comprime con la menor pérdida posible.

    /// <summary>
    ///  Convierte de Bitmap a formato WSQ
    /// </summary>
    /// <param name="bmp">Imagen que se desea convertir</param>
    /// <param name="context">Contexto del WSQ</param>
    /// <returns>Retorna un arreglo de bytes con la imagen en formato WSQ</returns>
    public static byte[] BitmapToWSQ(Bitmap bmp, IntPtr context)
	
	
	/// <summary>
    ///  Convierte de Bitmap a formato WSQ
    /// </summary>
    /// <param name="bmp">Imagen que se desea convertir</param>
    /// <returns>Retorna un arreglo de bytes con la imagen en formato WSQ</returns>
    public static byte[] BitmapToWSQ(Bitmap bmp)

	
	/// <summary>
    ///   Convierte un formato WSQ a formato Bitmap
    /// </summary>
    /// <param name="wsq">Arreglo que contiene la imagen en formato WSQ</param>
    /// <param name="context">Contexto del WSQ</param>
    /// <returns>Bitmap</returns>
    public static Bitmap WSQToBitmap(byte[] wsq, IntPtr context)

	
	
	/// <summary>
    ///   Convierte un formato WSQ a formato Bitmap
    /// </summary>
    /// <param name="wsq">Arreglo que contiene la imagen en formato WSQ</param>
    /// <returns>Bitmap</returns>
    public static Bitmap WSQToBitmap(byte[] wsq)

	
     using AFIS.WSQ;

        FileStream fs = File.OpenRead("finger.bmp");
        Console.WriteLine("Size of Bitmap Finger: " + fs.Length);
        fs.Close();
        Bitmap bmp = new Bitmap("finger.bmp");
        // Convierte de Bitmap a WSQ
        byte[] buffer = WSQ.BitmapToWSQ(bmp);

        //Convierte de WSQ a Bitmap
        Bitmap tmp = WSQ.WSQToBitmap(buffer);

        Console.WriteLine("Size of WSQ Finger: " + buffer.Length);
        tmp.Save("finger-2.bmp", ImageFormat.Bmp);
        fs = File.Create("finger.wsq");
        fs.Write(buffer, 0, buffer.Length);
        fs.Close();
	
Probado su funcionamiento en entornos multi hilos (thread safe)

The non-export controlled NBIS software includes five major packages: (PCASYS, MINDTCT, NFIQ, AN2K7, and IMGTOOLS). 