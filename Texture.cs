using System;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System.IO;

public class Texture : IDisposable
{
    public int Handle { get; private set; }

    public Texture(string path)
    {
        // 1. Генерируем пустой объект текстуры на GPU
        Handle = GL.GenTexture();

        // 2. Активируем текстурный юнит и привязываем текстуру
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Handle);

        // StbImageSharp по умолчанию загружает изображения сверху вниз, 
        // а OpenGL ожидает снизу вверх. Переворачиваем:
        StbImage.stbi_set_flip_vertically_on_load(1);

        // 3. Загружаем пиксели из файла
        using (Stream stream = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            // 4. Копируем данные пикселей в память видеокарты
            GL.TexImage2D(TextureTarget.Texture2D, 
                          0, 
                          PixelInternalFormat.Rgba, 
                          image.Width, 
                          image.Height, 
                          0, 
                          PixelFormat.Rgba, 
                          PixelType.UnsignedByte, 
                          image.Data);
        }

        // 5. Настраиваем параметры растяжения (S и T координаты — аналог X и Y)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // 6. Настраиваем фильтрацию (размытие при изменении масштаба)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // 7. (Опционально) Генерируем мипмапы для оптимизации на расстоянии
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    // Метод для активации текстуры перед рендерингом
    public void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }

    public void Dispose()
    {
        GL.DeleteTexture(Handle);
        GC.SuppressFinalize(this);
    }
}
