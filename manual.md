# Manual de Uso: Calculadora de Tensões em Serviço (ELS) para Elementos Protendidos (Versão Aprimorada)

**Objetivo:**

Esta calculadora tem como finalidade auxiliar na verificação das tensões em seções de elementos estruturais de concreto protendido (como vigas pré-moldadas e mistas) no Estado Limite de Serviço (ELS). Os cálculos e as verificações de tensões seguem as premissas e critérios da norma **ABNT NBR 6118:2023**, com foco particular no item 17.2.4.3.2 (a) para os limites de tensão no concreto.

**Convenção de Sinais:**

*   **Tensões:** Compressão é **negativa (-)**, Tração é **positiva (+)**.
*   **Momentos Fletores Externos (M<sub>g</sub>, M<sub>q</sub>):** Positivos quando tracionam as fibras inferiores da seção (convenção usual da análise estrutural para flexão simples).
*   **Esforços Normais Externos (N<sub>g</sub>, N<sub>q</sub>):** Forças de compressão são **negativas (-)**, forças de tração são **positivas (+)**.

**Unidades:**

*   **Entrada de Dados:** Utilizar as unidades indicadas nos rótulos de cada campo (geralmente tf para força, m para dimensão, tf.m para momento).
*   **Resultados:**
    *   Tensões (individuais, combinadas e limites): Apresentadas em **tf/m²**.
    *   Propriedades geométricas: Apresentadas em **m, m², m³, m⁴**.

**Navegação:**

A interface da calculadora é organizada em três abas principais, que representam etapas de entrada de dados:

1.  **1. Geometria:** Definição da seção transversal da viga e elementos compostos.
2.  **2. Materiais e Protensão:** Propriedades do concreto e detalhes das armaduras de protensão.
3.  **3. Ações e Coeficientes:** Momentos fletores, esforços normais externos e coeficientes de combinação ψ.

Utilize os botões das abas no topo da seção de entrada ou os botões de navegação "‹ Voltar" e "Próximo ›" localizados abaixo dos campos de entrada para alternar entre as etapas.

---

**Entrada de Dados Detalhada:**

**Aba 1: Geometria**

*   **1.1 Viga Pré-Moldada:**
    *   Define a geometria da viga principal (geralmente pré-moldada). Insira as dimensões de cada elemento retangular ou trapezoidal que compõe a viga, **começando pelo elemento mais inferior e seguindo para cima**.
    *   **Elem.:** Identificador numérico do elemento (gerado automaticamente).
    *   **B inf. (m):** Largura da base inferior do elemento da viga.
    *   **B sup. (m):** Largura da base superior do elemento da viga. Para elementos retangulares, B sup. = B inf.
    *   **h (m):** Altura do elemento da viga.
    *   **Área (m²):** Calculada automaticamente para cada elemento.
    *   Use os botões **`+`** (Adicionar Elemento) e **`-`** (Remover Último Elemento) para gerenciar os elementos da viga.
*   **1.2 Laje/Capa (Composta):**
    *   Define as dimensões das partes da seção que atuarão em conjunto com a viga principal para formar a seção composta final. Se não houver seção composta (ex: laje ou capa de solidarização), mantenha os valores dimensionais como 0.
    *   **Laje Pré (1ª Camada):**
        *   **Largura (b<sub>f1</sub>) (m):** Largura colaborante da primeira camada da laje (ex: pré-laje).
        *   **Altura (h<sub>f1</sub>) (m):** Espessura da primeira camada da laje.
    *   **Capa Conc. (2ª Camada):**
        *   **Largura (b<sub>f2</sub>) (m):** Largura colaborante da segunda camada da laje (ex: capa de concreto moldada no local).
        *   **Altura (h<sub>f2</sub>) (m):** Espessura da segunda camada da laje.

**Aba 2: Materiais e Protensão**

*   **2.1 Concreto:**
    *   **f<sub>ck,j</sub> (ATO) (tf/m²):** Resistência característica à compressão do concreto da viga na idade `j` de aplicação da protensão (ATO - Ato da Protensão). Usada para determinar os limites de tensão no concreto (compressão e tração) durante as fases iniciais de protensão.
    *   **f<sub>ck,j</sub> (Interm.) (tf/m²):** Resistência característica à compressão do concreto da viga na idade `j` correspondente à aplicação de cargas intermediárias (como G2 e P2, se aplicável antes da cura completa da seção composta). Usada para o limite de tração `α⋅f_ctm,j` nas verificações intermediárias.
    *   **f<sub>ck</sub> (28d) (tf/m²):** Resistência característica à compressão do concreto da viga aos 28 dias. Usada para os limites de compressão em serviço (ELS) e para o cálculo de `f_ctk,inf` (limite de tração para ELS Frequente).
    *   **α (Alfa):** Coeficiente que majora a resistência à tração média do concreto (`f_ctm`) para obtenção do limite de tração em certas fases (NBR 6118:2023, Tabela 17.2).
        *   O usuário deve determinar `α` com base na relação `b/b_p` (largura da zona tracionada / largura da zona de contato da ancoragem da protensão), conforme item 17.2.4.3.2 da NBR 6118:
            *   α = 1.5 para b/b<sub>p</sub> ≤ 5
            *   α = 1.2 para 5 < b/b<sub>p</sub> < 8
            *   α = 1.0 para b/b<sub>p</sub> ≥ 8
        *   A calculadora usa o valor inserido diretamente.
*   **2.2 Protensão:**
    *   Permite definir até duas etapas de protensão. Se a "Etapa 2" não for utilizada, mantenha seus campos como 0.
    *   **Hipótese:** A Protensão Etapa 2 (P<sub>0,2</sub>) é aplicada sobre a seção inicial da viga (sem lajes), após o carregamento G2.
    *   **CG Cabos (m):** Distância vertical da face inferior da viga pré-moldada (base do primeiro elemento geométrico) até o centro de gravidade (CG) dos cabos de protensão para a etapa correspondente.
    *   **Nº Cabos (unid.):** Número total de cabos ou cordoalhas idênticos aplicados na etapa.
    *   **P<sub>0</sub> (tf/cabo):** Força de protensão característica *por cabo* após perdas imediatas na ancoragem (antes das demais perdas iniciais e progressivas).
    *   **P<sub>∞</sub> (tf/cabo):** Força de protensão final *por cabo* após todas as perdas (imediatas e progressivas).
    *   **% P<sub>0</sub> (ATO) (%):** Percentual da força inicial P<sub>0</sub> (da Etapa 1) a ser considerado na verificação da Combinação 0 (ATO Parcial). Usualmente 100%.

**Aba 3: Ações e Coeficientes**

*   **3.1 Momentos Solicitantes (tf.m):**
    *   **M<sub>g1</sub>:** Momento fletor devido à carga permanente G1 (ex: peso próprio da viga), solicitando a seção inicial.
    *   **M<sub>g2</sub>:** Momento fletor devido à carga permanente G2 (ex: peso da pré-laje e capa úmida), solicitando a seção inicial.
    *   **M<sub>g3</sub>:** Momento fletor devido à carga permanente G3 (ex: revestimentos, pavimentação), solicitando a seção composta final.
    *   **M<sub>q</sub>:** Momento fletor devido à carga variável principal (Q), solicitando a seção composta final.
    *   **M<sub>p...</sub> (calc.):** Momentos fletores gerados pela excentricidade das forças de protensão. São calculados automaticamente e exibidos para conferência.
*   **3.2 Esforços Normais Solicitantes (tf):**
    *   Informe os esforços normais externos aplicados à seção (Compressão negativa, Tração positiva).
    *   **N<sub>g1</sub>, N<sub>g2</sub>:** Esforços normais externos concomitantes com M<sub>g1</sub> e M<sub>g2</sub> (atuam na seção inicial).
    *   **N<sub>g3</sub>:** Esforço normal externo concomitante com M<sub>g3</sub> (atua na seção final).
    *   **N<sub>q</sub>:** Esforço normal externo concomitante com M<sub>q</sub> (atua na seção final).
    *   **N<sub>p...</sub> (calc.):** Forças normais de protensão totais. São calculadas automaticamente com base nos dados de P<sub>0</sub>, P<sub>∞</sub> e Nº de Cabos, e exibidas com sinal negativo (compressão).
*   **3.3 Coeficientes (ψ):**
    *   **ψ<sub>1</sub>:** Fator de redução para combinação frequente da carga variável principal (M<sub>q</sub>, N<sub>q</sub>).
    *   **ψ<sub>2</sub>:** Fator de redução para combinação quase-permanente da carga variável principal (M<sub>q</sub>, N<sub>q</sub>).

---

**Botão "Calcular Tensões":**

*   Localizado na coluna da direita, abaixo das "Observações" e da "Visualização da Seção".
*   Clique neste botão após o preenchimento completo e correto de todos os dados de entrada nas três abas para executar os cálculos e exibir os resultados.

---

**Resultados:**

Após o cálculo, uma nova seção de "Resultados" é exibida abaixo da área de entrada, contendo as seguintes informações:

*   **Propriedades Geométricas:**
    *   São apresentadas as propriedades para a **Seção Inicial** (viga pré-moldada isolada) e para a **Seção Final** (viga + laje/capa, ou seja, seção composta).
    *   Valores incluem: Área (A), Posição do CG a partir da base (Y<sub>inf</sub>), Posição do CG a partir do topo (Y<sub>sup</sub> viga ou Y<sub>sup</sub> total), Momento de Inércia (I), e Módulos de Resistência Elásticos (W<sub>inf</sub>, W<sub>sup1</sub> para topo da viga, W<sub>sup2</sub> para topo da capa).
*   **Tensões Individuais por Ação (tf/m²):**
    *   Detalha a contribuição de cada parcela de carregamento para as tensões nas fibras críticas da seção:
        *   σ<sub>inf</sub>: Tensão na fibra inferior da viga.
        *   σ<sub>sup1</sub>: Tensão na fibra superior da viga pré-moldada.
        *   σ<sub>sup2</sub>: Tensão na fibra superior da capa (se houver seção composta).
    *   Ações consideradas: M<sub>g1</sub>, M<sub>g2</sub>, M<sub>g3</sub>, M<sub>q</sub>, N<sub>g1</sub>, N<sub>g2</sub>, N<sub>g3</sub>, N<sub>q</sub>, e os efeitos axiais e de flexão da Protensão P<sub>1o</sub>, P<sub>2o</sub>, P<sub>1∞</sub>, P<sub>2∞</sub>.
*   **Tensões Limites (tf/m²):**
    *   Apresenta os valores limites normativos de tensão (compressão e tração) para cada fibra relevante em cada uma das 6 combinações de verificação (0 a 5). Estes limites são calculados com base nas resistências do concreto (f<sub>ck</sub>'s), no coeficiente α e nos critérios da NBR 6118:2023 (17.2.4.3.2a).
    *   **Verif. 0 (ATO %P<sub>0</sub>):**
        *   σ<sub>inf</sub> (Comp.): ≥ -0.7⋅f<sub>ck,j(ATO)</sub>
        *   σ<sub>sup1</sub> (Traç.): ≤ α⋅f<sub>ctm,j(ATO)</sub>
    *   **Verif. 1 (ATO 100% P<sub>0</sub>):**
        *   σ<sub>inf</sub> (Comp.): ≥ -0.7⋅f<sub>ck,j(ATO)</sub>
        *   σ<sub>sup1</sub> (Traç.): ≤ α⋅f<sub>ctm,j(ATO)</sub>
    *   **Verif. 2 (Interm. G1+G2+P<sub>1o</sub>):** Considera P1o na seção inicial.
        *   σ<sub>inf</sub> (Traç.): ≤ α⋅f<sub>ctm,j(Interm)</sub>
        *   σ<sub>sup1</sub> (Comp.): ≥ -0.7⋅f<sub>ck(28d)</sub>
    *   **Verif. 3 (Interm. G1+G2+P<sub>1o</sub>+P<sub>2o</sub>):** Considera P1o e P2o na seção inicial. σ<sub>sup2</sub> não aplicável.
        *   σ<sub>inf</sub> (Comp.): ≥ -0.7⋅f<sub>ck(28d)</sub>
        *   σ<sub>sup1</sub> (Traç.): ≤ α⋅f<sub>ctm,j(Interm)</sub>
    *   **Verif. 4 (ELS Frequente):**
        *   σ<sub>inf</sub> (Traç.): ≤ f<sub>ctk,inf</sub> (calculado com f<sub>ck(28d)</sub>)
        *   σ<sub>sup2</sub> (Comp.): ≥ -0.60⋅f<sub>ck(28d)</sub>
    *   **Verif. 5 (ELS Quase Permanente / Descompressão):**
        *   σ<sub>inf</sub> (Traç.): ≤ 0 (Descompressão Limite)
        *   σ<sub>sup2</sub> (Comp.): ≥ -0.45⋅f<sub>ck(28d)</sub>
*   **Tensões Calculadas (tf/m²):**
    *   Exibe as tensões finais combinadas para cada uma das 6 verificações, resultantes da soma algébrica das tensões individuais apropriadas para cada combinação e fibra.
    *   **Comb. 0 (ATO %P<sub>0</sub>):** [M<sub>g1</sub> + N<sub>g1</sub>]<sub>Sec.Inicial</sub> + (%P<sub>0</sub>/100)⋅[P<sub>1o</sub>]<sub>Sec.Inicial</sub>
    *   **Comb. 1 (ATO 100% P<sub>0</sub>):** [M<sub>g1</sub> + N<sub>g1</sub>]<sub>Sec.Inicial</sub> + [P<sub>1o</sub>]<sub>Sec.Inicial</sub>
    *   **Comb. 2 (Interm. 1):** [M<sub>g1</sub>+M<sub>g2</sub> + N<sub>g1</sub>+N<sub>g2</sub>]<sub>Sec.Inicial</sub> + [P<sub>1o</sub>]<sub>Sec.Inicial</sub>
    *   **Comb. 3 (Interm. 2):** [M<sub>g1</sub>+M<sub>g2</sub> + N<sub>g1</sub>+N<sub>g2</sub>]<sub>Sec.Inicial</sub> + [P<sub>1o</sub>+P<sub>2o</sub>]<sub>Sec.Inicial</sub>
    *   **Comb. 4 (ELS Frequente):** [ΣM<sub>G</sub>+ΣN<sub>G</sub>] + [ΣP<sub>∞</sub>]<sub>Sec.Final</sub> + ψ<sub>1</sub>⋅[M<sub>q</sub>+N<sub>q</sub>]<sub>Sec.Final</sub>
        *   *Onde as tensões de G1, G2, N<sub>g1</sub>, N<sub>g2</sub> (originalmente na seção inicial) são somadas às de G3, N<sub>g3</sub> (na seção final), e P<sub>∞</sub> atua na seção final.*
    *   **Comb. 5 (ELS Quase Permanente):** [ΣM<sub>G</sub>+ΣN<sub>G</sub>] + [ΣP<sub>∞</sub>]<sub>Sec.Final</sub> + ψ<sub>2</sub>⋅[M<sub>q</sub>+N<sub>q</sub>]<sub>Sec.Final</sub>
    *   **Verif.:** Indica o status da verificação para cada combinação e conjunto de fibras:
        *   **<span style="color:green;">OK</span>**: A tensão calculada atende ao limite normativo.
        *   **<span style="color:red;">FALHA</span>**: A tensão calculada excede o limite normativo.

---

**Visualização da Seção:**

*   Ao lado da área de entrada de dados, um desenho esquemático da seção transversal (viga e laje/capa, se houver) é exibido dinamicamente conforme os dados geométricos são inseridos.
*   Os elementos da viga são numerados, e a posição dos CGs das etapas de protensão (CG P1, CG P2) é indicada por círculos coloridos, se definidos.

---

**Observações Importantes:**

*   **Ordem de Inserção:** Lembre-se de inserir os elementos que compõem a viga pré-moldada na tabela de geometria **de baixo para cima**.
*   **Consistência:** Verifique cuidadosamente a consistência entre as unidades de entrada, as convenções de sinal e os estágios de aplicação das cargas e da protensão.
*   **Hipóteses de Cálculo:**
    *   A protensão da Etapa 2 (P<sub>0,2</sub>) é considerada aplicada sobre a seção inicial da viga (sem a contribuição das lajes/capa).
    *   Nas combinações finais (ELS-F, ELS-D), as forças de protensão finais (P<sub>1∞</sub>, P<sub>2∞</sub>) são consideradas atuando na seção composta final.
    *   A análise de tensões é linear elástica.
*   **Referência Normativa:** Os critérios de combinação e limites de tensão buscam seguir as prescrições da ABNT NBR 6118:2023. Recomenda-se sempre a consulta direta à norma para entendimento completo e casos específicos.
