namespace CasaDeAxe.Domain.Entities
{
    public class TextoPonto
    {
        public int Id { get; set; } // Identificador único
        public string TituloDoponto { get; set; } // Titulo do ponto gravado
        public string LinkDoYouTube { get; set; } // Link para o vídeo do YouTube
        public string LetraDoPonto { get; set; } //Letra do ponto
        public string CategoriaDoPontos { get; set; } // Categoria do Ponto
    }
}
