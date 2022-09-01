using tabuleiro;

namespace xadrez
{
    internal class PartidaDeXadrez
    {
        public Tabuleiro tabuleiro { get; private set; }
        private int turno;
        private Cor jogadorAtual;
        public bool terminada { get; private set; }

        public PartidaDeXadrez()
        {
            tabuleiro = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            ColocarPecas();
        }

        public void ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca peca = tabuleiro.RetirarPeca(origem);
            peca.IncrementarQteMovimentos();
            Peca pecaCapturada = tabuleiro.RetirarPeca(destino);
            tabuleiro.ColocarPeca(peca, destino);
        }
        private void ColocarPecas()
        {
            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Branca), new PosicaoXadrez('c', 1).ToPosicao());
            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Branca), new PosicaoXadrez('c', 2).ToPosicao());
            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Branca), new PosicaoXadrez('d', 2).ToPosicao());
            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Branca), new PosicaoXadrez('e', 2).ToPosicao());
            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Branca), new PosicaoXadrez('e', 1).ToPosicao());
            tabuleiro.ColocarPeca(new Rei(tabuleiro, Cor.Branca), new PosicaoXadrez('d', 1).ToPosicao());

            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Preta), new PosicaoXadrez('c', 8).ToPosicao());
            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Preta), new PosicaoXadrez('c', 7).ToPosicao());
            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Preta), new PosicaoXadrez('d', 7).ToPosicao());
            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Preta), new PosicaoXadrez('e', 7).ToPosicao());
            tabuleiro.ColocarPeca(new Torre(tabuleiro, Cor.Preta), new PosicaoXadrez('e', 8).ToPosicao());
            tabuleiro.ColocarPeca(new Rei(tabuleiro, Cor.Preta), new PosicaoXadrez('d', 8).ToPosicao());
        }
    }
}
