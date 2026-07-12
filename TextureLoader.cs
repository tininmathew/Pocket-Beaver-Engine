using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System.IO;

namespace Engine;

public static class TextureLoader
{
    public static int LoadTexture(string path)
    {
        // 1. Создаем объект текстуры в видеопамяти
        int handle = GL.GenTexture();
        
        // 2. Делаем текстуру активной (привязываем к типу Texture2D)
        GL.BindTexture(TextureTarget.Texture2D, handle);

        // 3. Настраиваем параметры фильтрации и повторения
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // Переворачиваем картинку по вертикали (OpenGL отсчитывает Y снизу вверх)
        StbImage.stbi_set_flip_vertically_on_load(1);

        if(!File.Exists(path)) 
        {
            path = "resources/fallback texture error.bmp";
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        // 4. Загружаем пиксели из файла в оперативную память
        using (Stream stream = File.OpenRead(path))
        {
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            
            // 5. Копируем массив пикселей из ОЗУ в видеопамять (GPU)
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

        // 6. Генерируем Mip-уровни для оптимизации отображения вдали
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        // Отвязываем текстуру
        GL.BindTexture(TextureTarget.Texture2D, 0);

        return handle; // Возвращаем ID текстуры для дальнейшего рендеринга
    }
}