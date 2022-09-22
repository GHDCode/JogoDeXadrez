using tabuleiro;

namespace xadrez
{
    internal class PartidaDeXadrez
    {
        public Tabuleiro tabuleiro { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;
        public bool xeque { get; private set; }
        public Peca vulneravelEnPassant { get; private set; }
        public PartidaDeXadrez()
        {
            tabuleiro = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            xeque = false;
            vulneravelEnPassant = null;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            ColocarPecas();
        }

        public Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca peca = tabuleiro.RetirarPeca(origem);
            peca.IncrementarQteMovimentos();
            Peca pecaCapturada = tabuleiro.RetirarPeca(destino);
            tabuleiro.ColocarPeca(peca, destino);
            if (pecaCapturada != null)
            {
                capturadas.Add(pecaCapturada);
            }

            //# Jogada Especial: Roque Pequeno
            if (peca is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca t = tabuleiro.RetirarPeca(origemT);
                t.IncrementarQteMovimentos();
                tabuleiro.ColocarPeca(t, destinoT);
            }
            //# Jogada Especial: Roque Grande
            if (peca is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca t = tabuleiro.RetirarPeca(origemT);
                t.IncrementarQteMovimentos();
                tabuleiro.ColocarPeca(t, destinoT);
            }

            //#Jogada Especial: En Passant
            if (peca is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == null)
                {
                    Posicao posP;
                    if (peca.Cor == Cor.Branca)
                    {
                        posP = new Posicao(destino.Linha + 1, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(destino.Linha - 1, destino.Coluna);
                    }
                    pecaCapturada = tabuleiro.RetirarPeca(posP);
                    capturadas.Add(pecaCapturada);
                }
            }
            return pecaCapturada;
        }
        public void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tabuleiro.RetirarPeca(destino);
            p.DecrementarQteMovimentos();
            if (pecaCapturada != null)
            {
                tabuleiro.ColocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }
            tabuleiro.ColocarPeca(p, origem);

            //# Jogada Especial: Roque Pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca t = tabuleiro.RetirarPeca(destinoT);
                t.DecrementarQteMovimentos();
                tabuleiro.ColocarPeca(t, origemT);
            }

            //# Jogada Especial: Roque Grande
            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca t = tabuleiro.RetirarPeca(destinoT);
                t.DecrementarQteMovimentos();
                tabuleiro.ColocarPeca(t, origemT);
            }

            //Jogada Especial: En Passant 
            if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == vulneravelEnPassant)
                {
                    Peca peao = tabuleiro.RetirarPeca(destino);
                    Posicao posP;
                    if (p.Cor == Cor.Branca)
                    {
                        posP = new Posicao(3, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(4, destino.Coluna);
                    }
                    tabuleiro.ColocarPeca(peao, posP);
                }
            }
        }
        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);

            if (EstaEmXeque(jogadorAtual))
            {
                DesfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }
            if (EstaEmXeque(Adversaria(jogadorAtual)))
            {
                xeque = true;
            }
            else
            {
                xeque = false;
            }
            if (TesteXequeMate(Adversaria(jogadorAtual)))
            {
                terminada = true;
            }
            else
            {
                turno++;
                MudaJogador();
            }

            Peca p = tabuleiro.Peca(destino);

            //#Jogada Especial: En Passant
            if (p is Peao && (destino.Linha == origem.Linha - 2 || destino.Linha == origem.Linha + 2))
            {
                vulneravelEnPassant = p;
            }
            else
            {
                vulneravelEnPassant = null;
            }
        }
        public void ValidarPosicaoDeOrigem(Posicao pos)
        {
            if (tabuleiro.Peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            if (jogadorAtual != tabuleiro.Peca(pos).Cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }
            if (!tabuleiro.Peca(pos).ExisteMovimentosPossiveis())
            {
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem escolhida!");
            }
        }
        public void ValidarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!tabuleiro.Peca(origem).MovimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de destino inválida!");
            }
        }
        private void MudaJogador()
        {
            if (jogadorAtual == Cor.Branca)
            {
                jogadorAtual = Cor.Preta;
            }
            else
            {
                jogadorAtual = Cor.Branca;
            }
        }
        public HashSet<Peca> PecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in capturadas)
            {
                if (x.Cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }
        public HashSet<Peca> PecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in pecas)
            {
                if (x.Cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(PecasCapturadas(cor));
            return aux;
        }
        private Cor Adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            }
            else
            {
                return Cor.Branca;
            }
        }
        private Peca Rei(Cor cor)
        {
            foreach (Peca x in PecasEmJogo(cor))
            {
                if (x is Rei)
                {
                    return x;
                }

            }
            return null;
        }
        public bool EstaEmXeque(Cor cor)
        {
            Peca r = Rei(cor); 
            if (r == null)
            {
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro!");
            }
            foreach (Peca x in PecasEmJogo(Adversaria(cor)))
            {
                bool[,] mat = x.MovimentosPossiveis();
                if (mat[r.Posicao.Linha, r.Posicao.Coluna])
                {
                    return true;
                }
            }
            return false;
            
        }
        public bool TesteXequeMate(Cor cor)
        {
            if (!EstaEmXeque(cor))
            {
                return false;
            }
            foreach (Peca x in PecasEmJogo(cor))
            {
                bool[,] mat = x.MovimentosPossiveis();

                for (int i = 0; i < tabuleiro.Linhas; i++)
                {
                    for (int j = 0; j < tabuleiro.Colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = ExecutaMovimento(origem , destino);
                            bool testeXeque = EstaEmXeque(cor);
                            DesfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tabuleiro.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            pecas.Add(peca);
        }
        private void ColocarPecas()
        {
            ColocarNovaPeca('a', 1, new Torre(tabuleiro, Cor.Branca));
            ColocarNovaPeca('b', 1, new Cavalo(tabuleiro, Cor.Branca));
            ColocarNovaPeca('c', 1, new Bispo(tabuleiro, Cor.Branca));
            ColocarNovaPeca('d', 1, new Dama(tabuleiro, Cor.Branca));
            ColocarNovaPeca('e', 1, new Rei(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('f', 1, new Bispo(tabuleiro, Cor.Branca));
            ColocarNovaPeca('g', 1, new Cavalo(tabuleiro, Cor.Branca));
            ColocarNovaPeca('h', 1, new Torre(tabuleiro, Cor.Branca));
            ColocarNovaPeca('a', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('b', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('c', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('d', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('e', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('f', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('g', 2, new Peao(tabuleiro, Cor.Branca, this));
            ColocarNovaPeca('h', 2, new Peao(tabuleiro, Cor.Branca, this));

            ColocarNovaPeca('a', 8, new Torre(tabuleiro, Cor.Preta));
            ColocarNovaPeca('b', 8, new Cavalo(tabuleiro, Cor.Preta));
            ColocarNovaPeca('c', 8, new Bispo(tabuleiro, Cor.Preta));
            ColocarNovaPeca('d', 8, new Dama(tabuleiro, Cor.Preta));
            ColocarNovaPeca('e', 8, new Rei(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('f', 8, new Bispo(tabuleiro, Cor.Preta));
            ColocarNovaPeca('g', 8, new Cavalo(tabuleiro, Cor.Preta));
            ColocarNovaPeca('h', 8, new Torre(tabuleiro, Cor.Preta));
            ColocarNovaPeca('a', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('b', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('c', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('d', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('e', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('f', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('g', 7, new Peao(tabuleiro, Cor.Preta, this));
            ColocarNovaPeca('h', 7, new Peao(tabuleiro, Cor.Preta, this));

        }
    }
}
