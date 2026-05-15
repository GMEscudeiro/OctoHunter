# Mecânica

## Elementos Formais do Jogo

### Padrão de Interação do Jogador
O jogador interage com o jogo por meio de **teclado e mouse**. As principais ações são:

- **Movimentação:**
  - **W, A, S, D:** Movimentam o Polvo pelos quatro eixos no ambiente 2D.
- **Combate:**
  - **Clique Esquerdo do Mouse:** Dispara ou usa a arma equipada atualmente.
- **Gestão de Arsenal:**
  - **Barra de Armas (clique nos slots):** Troca a arma ativa entre as até 8 armas carregadas simultaneamente.
- **Interface:**
  - O cursor do mouse é utilizado para navegar nos menus do Cassino, selecionar itens e confirmar compras.

### Objetivo do Jogo

#### Quando o Jogador Ganha?
O jogador vence quando derrota os **chefões das três espécies** — Cobra, Aranha e Escorpião — e coleta todos os **fragmentos da nave**. Ao completar esse objetivo, o Polvo repara sua nave e parte do planeta, concluindo sua missão.

#### Quando o Jogador Perde?
O jogador perde quando o Polvo fica **sem vidas**. O jogo retorna ao menu inicial, onde o jogador pode iniciar uma nova sessão do zero — sem carregamento de progresso entre partidas.

### Regras do Jogo

- **Estrutura de Rounds:**
  - Cada espécie possui **3 rounds normais** (com 3 hordas cada) seguidos de **1 round de chefão**.
  - Ao fim de cada round (normal ou chefão), o jogador é levado ao Cassino antes do próximo round.
  - As espécies são embaralhadas aleatoriamente a cada sessão.

- **Hordas:**
  - Em cada round normal, **3 hordas** de inimigos surgem em sequência ao redor do Polvo.
  - O número de inimigos por horda escala com o total de rounds completados na sessão.
  - A próxima horda só começa após todos os inimigos da horda anterior serem eliminados.

- **Vidas:**
  - O Polvo possui **3 vidas** por padrão.
  - Ao sofrer dano, ele entra em **invencibilidade temporária** por 1,5 segundos.
  - Armas do tipo Escudo absorvem um hit antes de consumir uma vida.

- **Moedas:**
  - Inimigos derrotados dropam moedas. Chefões dropam quantidades maiores.
  - As moedas são usadas exclusivamente no Cassino para comprar armas e fazer rerolls.
  - O saldo de moedas é zerado ao iniciar uma nova sessão.

- **Arsenal:**
  - O jogador começa com a **Pistola** como arma inicial.
  - Novas armas são adquiridas no Cassino. O inventário suporta até **8 armas**.
  - Com o inventário cheio, comprar uma nova arma exige **trocar ou vender** uma existente.
  - Vender uma arma retorna **metade do seu preço original** em moedas.
  - O Cassino oferece **3 armas por visita**, sorteadas com chances ponderadas por raridade (Comum 60%, Raro 30%, Épico 10%).
  - O sortimento pode ser **rerrollado** pelo custo de 5 moedas.

- **Chefões:**
  - Cada espécie possui um chefão com padrões de ataque únicos e saúde escalada pelo progresso da sessão.
  - Ao derrotar o chefão, **2 fragmentos da nave** são dropados e precisam ser coletados pelo jogador.

### Procedimentos do Jogo

1. **Tutorial (opcional):** O jogador aprende a se movimentar, atirar e trocar armas com orientação de Leroy.
2. **Início de Sessão:** O inventário é resetado com apenas a Pistola. As moedas são zeradas. As espécies são embaralhadas.
3. **Round Normal:** Três hordas de inimigos surgem em sequência. Ao derrotar todas, o round encerra.
4. **Cassino:** O jogador gasta moedas para comprar armas, fazer rerolls ou vender itens do inventário antes de seguir.
5. **Round de Chefão (Round 3 de cada espécie):** O chefão surge isolado. Ao ser derrotado, dropa os fragmentos da nave e uma cutscene de vitória é exibida.
6. **Próxima Espécie:** Após o chefão, o ciclo recomeça na próxima espécie da ordem embaralhada.
7. **Vitória ou Derrota:** Ao derrotar os três chefões, a vitória é declarada. Ao perder todas as vidas, o jogo encerra e retorna ao menu.

### Recursos do Jogo

- **Arsenal de 10 Armas:**

  | Arma | Tipo | Efeito Especial |
  |---|---|---|
  | Pistola | Distância | — |
  | Escopeta | Distância (área) | Múltiplos projéteis |
  | Mac10 | Distância (rápido) | Alta cadência |
  | Lança-Chamas | Distância (contínuo) | Aplica queimadura |
  | Pistola de Gelo | Distância | Aplica congelamento |
  | Katana | Corpo a corpo | Dano em arco |
  | Bazuca | Distância (explosivo) | Dano em área |
  | Nunchaku | Corpo a corpo | — |
  | Ímã | Suporte | Atrai moedas automaticamente |
  | Escudo | Suporte/Defesa | Absorve 1 hit por carga |

- **Sistema de Moedas:** Coletadas em combate, usadas no Cassino para comprar e melhorar o arsenal.
- **Fragmentos da Nave:** Coletados ao derrotar chefões; marcam o progresso da recuperação da nave.
- **Leroy (NPC):** Comenta eventos do jogo com diálogos reativos durante o combate e nas transições.

### Limites do Jogo

- **Limites de Inventário:** Máximo de 8 armas simultâneas; uma deve ser sacrificada para cada nova compra após o limite.
- **Limites de Progressão:** Três espécies com quatro rounds cada; o jogo termina após a derrota dos três chefões ou a perda de todas as vidas.
- **Limites de Reroll:** Cada reroll custa 5 moedas; sem moedas suficientes, o sortimento não pode ser alterado.
- **Limites de Recursos:** Moedas e inventário são zerados a cada nova sessão, sem progressão permanente entre partidas.

### Resultados do Jogo

#### Resultado Após a Vitória
Ao derrotar o terceiro chefão e coletar os últimos fragmentos da nave, uma sequência de diálogo de vitória com Leroy é exibida, celebrando a conclusão da missão. O jogo retorna ao menu inicial, permitindo uma nova sessão.

#### Resultado Após a Derrota
Quando o Polvo perde todas as vidas, uma sequência de diálogo de game over com Leroy é exibida. O jogo retorna ao menu inicial. Toda a progressão da sessão — moedas, arsenal, rounds completados — é perdida, e uma nova sessão começa do zero.
