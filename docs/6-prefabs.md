# Prefabs

## Polvo Caçador (Jogador)

**Nome:** Polvo Caçador

**Descrição:** Prefab do personagem controlado pelo jogador. Combina movimentação, mira por mouse, gerenciamento de vidas, carteira de moedas e suporte ao arsenal de armas.

**Quando são utilizados:** Instanciado em todas as cenas de combate (SnakeScene, SpiderScene, ScorpionScene) e na cena de tutorial.

**Quais seus componentes:**

- *Sprites:* Sprite do Polvo com animações de movimentação e disparo, configurados via `SpriteRenderer` e `Animator`.
- *Colisores:* `CircleCollider2D` no corpo principal; `Rigidbody2D` com gravidade zero para movimentação 2D top-down.
- *Scripts:*
  - `PlayerController` — Lê input WASD via `Rigidbody2D` e rotaciona o Polvo em direção ao cursor do mouse. Suporta paralisia temporária via `Paralyze(duration)`.
  - `PlayerHealth` — Gerencia as vidas do Polvo (padrão: 3). Aplica invencibilidade de 1,5 s após dano, flash vermelho de feedback visual, e escudo que absorve hits antes de consumir vidas. Dispara eventos estáticos `OnLivesChanged`, `OnLastHeart` e `OnPlayerDied`.
  - `WeaponManager` — Instancia e organiza os prefabs de arma do `WeaponInventory` ao redor do Polvo em círculo. Ouve `inventory.OnInventoryChanged` para recarregar o arsenal sempre que ele muda.
  - `PlayerWallet` — Mantém o saldo de moedas via `WalletData` (ScriptableObject). Expõe `AddCoins` e `SpendCoins`, e dispara o evento estático `OnCoinsChanged` para atualizar a HUD.
  - `PlayerEvents` — Ponte de eventos internos do jogador (ex.: pontuação por inimigo abatido) para outros sistemas.

---

## Nativos (Inimigos Comuns)

**Nome:** Nativos

**Descrição:** Conjunto de prefabs de inimigos comuns distribuídos em hordas durante os rounds normais. Existem variantes por espécie (Cobra, Aranha, Escorpião) e por tipo de ataque (melee ou ranged).

**Quando são utilizados:** Instanciados pelo `WaveSpawner` em pontos aleatórios ao redor do jogador no início de cada horda.

**Prefabs disponíveis:**
- `EnemySnake` / `EnemyRangedSnake` — Cobra melee e Cobra ranged (Bioma das Cobras)
- `EnemySpider` / `EnemyRangedSpider` — Aranha melee e Aranha ranged (Bioma das Aranhas)
- `EnemyScorpion` — Escorpião melee (Bioma dos Escorpiões)
- `EnemyProjectile` / `EnemyWhiteProjectile` — Projéteis disparados pelos inimigos ranged

**Quais seus componentes:**

- *Sprites:* Sprite animado de cada espécie, com cor base definida por `SpriteRenderer`. Flash vermelho aplicado por código no momento do dano.
- *Colisores:* `Collider2D` (trigger ou física) para detecção de dano melee; `NavMeshAgent` para pathfinding 2D.
- *Scripts:*
  - `Enemy` — Gerencia a vida do inimigo. Ao chegar a zero, dispara `OnEnemyDied` (estático, para contagem de horda) e `OnDied` (instância, para efeitos específicos do inimigo). Aplica efeitos de status recebidos via `HitData.Effect`.
  - `EnemyMovement` — Navega em direção ao jogador usando `NavMeshAgent` com fallback em linha reta. Inimigos ranged disparam `EnemyProjectile` em intervalos configuráveis (`attackRate`).
  - `EnemyMelee` — Aplica dano ao jogador por contato (`OnTriggerStay2D` / `OnCollisionStay2D`) com cooldown entre hits. Suporta efeitos especiais opcionais: paralisia (`appliesParalysis`) e infecção visual (`appliesInfection`).
  - `EnemyRanged` — Complementa `EnemyMovement` em inimigos ranged, controlando lógica de manutenção de distância.
  - `EnemyProjectile` — Projétil disparado pelo inimigo; move-se em linha reta e aplica dano e efeitos de status ao colidir com o jogador.
  - `CoinDrop` — Ao ser destruído, instancia entre `minCoins` e `maxCoins` prefabs de moeda espalhados ao redor da posição de morte.

---

## Chefões das Espécies

**Nome:** Chefões das Espécies

**Descrição:** Prefabs dos inimigos especiais que encerram cada bioma. Possuem maior quantidade de vida, padrões de ataque únicos e soltam moedas e fragmentos da nave ao serem derrotados.

**Quando são utilizados:** Instanciados pelo `WaveSpawner` no round de chefão (4º round de cada espécie).

**Prefabs disponíveis:**
- `SnakeBoos` — Chefão Cobra (Bioma das Cobras)
- `BossSpider_2` — Chefão Aranha (Bioma das Aranhas)
- `ScorpionBoss` — Chefão Escorpião (Bioma dos Escorpiões)

**Quais seus componentes:**

- *Sprites:* Sprite de maior porte que os inimigos comuns, com animações de ataque e morte.
- *Colisores:* `Collider2D` de corpo para colisão melee; `Rigidbody2D` para movimentação via código.
- *Scripts:*
  - `Enemy` — Base de vida e recebimento de dano, compartilhada com os inimigos comuns. O evento `OnDied` é escutado pelo script de boss para acionar a lógica de morte.
  - `BossMovement` — Controla o deslocamento do chefão entre os estados `Chasing` (persegue o jogador), `Retreating` (recua) e `Idle` (parado durante ataques).
  - `SnakeBoss` — Orquestra o loop de ataque da Cobra em sequência fixa: poça de veneno (`BossAbilityVenomPool`) → dash (`BossAbilityDash`) → anel de projéteis (`BossAbilityBulletRing`). Ao morrer, dropa moedas.
  - `SpiderBoss` — Orquestra o loop de ataque da Aranha com escolha aleatória entre dash, salto parabólico (`JumpAttack`) e projétil de paralisia. Ao morrer, dropa moedas.
  - `ScorpionBoss` — Orquestra o loop de ataque do Escorpião com dash ou salto. Configura o `EnemyMelee` para causar 2 de dano e aplicar infecção visual. Ao morrer, dropa moedas.
  - `BossAbilityVenomPool` — Spawna poças de veneno no chão ao redor do jogador.
  - `BossAbilityDash` — Executa um dash rápido em direção ao jogador via `Rigidbody2D`.
  - `BossAbilityBulletRing` — Dispara um anel de projéteis em todas as direções.
  - `EnemyMelee` — Aplica dano corpo a corpo com efeitos de status (infecção no Escorpião).

---

## Arsenal (Armas)

**Nome:** Arsenal

**Descrição:** Conjunto de prefabs de armas que o jogador equipa e carrega durante a sessão. Cada arma é instanciada como filho do Polvo pelo `WeaponManager` e posicionada ao redor dele em círculo.

**Quando são utilizados:** Instanciados pelo `WeaponManager` ao carregar o `WeaponInventory`. A Pistola é equipada no início; as demais são adquiridas no Cassino.

**Prefabs de armas disponíveis:**
- `Pistol` + `PistolProjectile`
- `Shotgun` + `ShotgunProjectile`
- `Mac10` + `Mac10Projectile`
- `Flamethrower` + `Flame`
- `IceGun` + `IceShard`
- `Katana`
- `Bazooka` + `Missile`
- `Baseballbet`
- `Magnet`
- `Shield`

**Quais seus componentes:**

- *Sprites:* Sprite visual de cada arma, posicionada ao redor do Polvo e rotacionada para apontar para fora.
- *Colisores:* Colisores nos projéteis (`Projectile`, `BazookaProjectile`, `FlamethrowerProjectile`, etc.) para detecção de hit nos inimigos.
- *Scripts:*
  - `WeaponBase` — Classe abstrata base de todas as armas. Controla `attackRate`, referência ao atacante e o loop de `PerformAttack()` via `Update`.
  - `WeaponInfo` — ScriptableObject com metadados da arma: nome, ícone, preço, raridade e descrição (exibidos no Cassino).
  - `WeaponEffect` / `WeaponInventory` — ScriptableObjects de efeito de status e inventário de armas obtidas.
  - `Pistol` — Arma inicial. Dispara um `PistolProjectile` por clique do mouse na direção do cursor.
  - `Shotgun` — Dispara múltiplos `ShotgunProjectile` em cone, cobrindo área frontal.
  - `Mac10` — Metralhadora leve; alta taxa de disparo (`attackRate` baixo) com projéteis únicos.
  - `Flamethrower` — Lança chamas contínuas via `FlamethrowerProjectile`; aplica `BurnStatus` nos inimigos atingidos.
  - `IceGun` — Dispara projéteis de gelo que aplicam `FreezeStatus`, reduzindo a velocidade do inimigo.
  - `Katana` — Arma corpo a corpo com dano em arco ao redor do Polvo.
  - `Bazooka` — Dispara `BazookaProjectile` que explode ao colidir, causando dano em área.
  - `BaseballBat` — Arma corpo a corpo sem efeito especial; bate em inimigos próximos.
  - `MagnetWeapon` — Passivo de suporte: a cada intervalo de `attackRate`, puxa todas as moedas no raio `pullRadius` em direção ao jogador.
  - `ShieldWeapon` — Passivo defensivo: ao ser inicializado, adiciona `bonusHits` ao escudo do `PlayerHealth`. Ao ser removido do inventário, remove as cargas.
  - `Projectile` — Classe base de projéteis do jogador: move-se em linha reta e chama `Enemy.TakeDamage(HitData)` ao colidir.
  - `HitData` — Struct que empacota dano, referência ao atacante e efeito de status para o sistema de dano.

---

## Leroy e Sistema de Diálogos

**Nome:** Leroy (NPC e Sistema de Diálogos)

**Descrição:** O sistema de diálogos gerencia as falas reativas de Leroy ao longo de toda a sessão. É composto por um singleton persistente entre cenas (`DialogueManager`) e um controlador de eventos por nível (`LevelDialogueController`).

**Quando são utilizados:** O `DialogueManager` persiste desde a StartScene até o fim da sessão. O `LevelDialogueController` reage a eventos de horda, morte de inimigos, perigo de vida e chefão derrotado em todas as cenas de combate.

**Quais seus componentes:**

- *Sprites:* Portrait de Leroy com expressões distintas por contexto, exibido dentro da `DialogueUI` via `Image` do Unity UI.
- *Colisores:* Nenhum — o sistema é puramente de interface.
- *Scripts:*
  - `DialogueManager` — Singleton `DontDestroyOnLoad` que sobrevive às trocas de cena. Centraliza `StartDialogue(DialogueData)` e `StopDialogue()`, delegando a exibição à `DialogueUI`.
  - `DialogueUI` — Controla a caixa de diálogo na tela: exibe sequências de `DialogueEntry` (texto + portrait), avança ao clicar em "Próximo" e encerra ao fim da sequência. Expõe o botão "Skip" para pular toda a sequência.
  - `LevelDialogueController` — Ouve eventos do jogo via delegates estáticos e dispara os diálogos adequados:
    - `WaveSpawner.OnHordeStarted` → diálogo de abertura de horda (apenas na 1ª horda do round)
    - `Enemy.OnEnemyDied` → diálogo de "poucos inimigos restantes" quando o contador cai abaixo do limiar
    - `WaveSpawner.OnBossPartsDropped` → diálogo de fragmentos da nave dropados
    - `PlayerHealth.OnLastHeart` → diálogo de perigo (último coração)
  - `DialogueData` — ScriptableObject com a lista de `DialogueEntry` que compõe uma sequência de falas de Leroy.
  - `DialogueEntry` — Estrutura com o texto da fala e o sprite do portrait de Leroy para aquela linha.
  - `CasinoDialogueManager` — Variante do sistema de diálogos para o contexto do Cassino, com bordas de caixa de fala no estilo cassino.
