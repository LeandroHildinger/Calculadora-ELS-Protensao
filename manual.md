# Manual do Usuário: Calculadora de Tensões em Serviço (ELS) para Elementos Estruturais Protendidos

## 1. Introdução

Bem-vindo ao manual do usuário da Calculadora de Tensões em Serviço (ELS) para Elementos Estruturais Protendidos. Esta ferramenta foi desenvolvida para auxiliar engenheiros civis e estudantes de engenharia na verificação de tensões em seções de vigas protendidas, considerando as etapas construtivas e as combinações de serviço conforme a NBR 6118:2023.

O objetivo principal é fornecer uma análise rápida e precisa das tensões normais em diferentes fibras da seção transversal (inferior, superior da viga pré-moldada e superior da seção composta), comparando-as com os limites normativos para diversas fases e combinações de carregamento.

Este manual detalha a interface do usuário, o processo de entrada de dados, a interpretação dos resultados e as premissas de cálculo adotadas.

## 2. Convenções e Premissas

Antes de utilizar a calculadora, é fundamental compreender as seguintes convenções e premissas:

* **Unidades:**
    * Dimensões geométricas: metros (m)
    * Forças: toneladas-força (tf)
    * Momentos: toneladas-força.metro (tf.m)
    * Resistência do concreto (fck): toneladas-força por metro quadrado (tf/m²)
    * Tensões: toneladas-força por metro quadrado (tf/m²)
* **Convenção de Sinais para Tensões:**
    * Compressão: Negativa (-)
    * Tração: Positiva (+)
* **Convenção de Sinais para Esforços Normais Externos (Ng1, Ng2, Ng3, Nq):**
    * Compressão: Negativa (-)
    * Tração: Positiva (+)
* **Referência Normativa Principal:** ABNT NBR 6118:2023 - Projeto de estruturas de concreto - Procedimento.
* **Critério de Tensões Limites:** Conforme item 17.2.4.3.2 (a) da NBR 6118:2023 para ELS.
* **Etapas Construtivas e de Protensão:**
    * Os elementos da viga pré-moldada são considerados de baixo para cima.
    * A protensão da Etapa 1 (P1o, P1∞) é aplicada à seção inicial (viga pré-moldada).
    * A protensão da Etapa 2 (P2o, P2∞) é introduzida após a aplicação do carregamento G2 e atua na seção inicial (antes da cura da capa, se aplicável para tensões imediatas) ou na seção composta para efeitos de longo prazo, conforme a lógica de cálculo das tensões individuais.
* **Seção Composta:** A calculadora considera a formação de uma seção composta pela viga pré-moldada e por até duas camadas de laje/capa concretadas posteriormente.
* **Cálculo de Propriedades Geométricas:** Realizado para a seção inicial (viga isolada) e para a seção final (composta).
* **Perdas de Protensão:** As forças de protensão iniciais ($P_o$) e finais após perdas ($P_\infty$) devem ser fornecidas pelo usuário.

## 3. Interface do Usuário

A interface da calculadora é dividida em três áreas principais: Seção de Entradas (com abas), Painel Lateral (Visualização e Observações) e Seção de Resultados.

### 3.1. Navegação por Abas

A entrada de dados é organizada em três abas sequenciais:

1.  **1. Geometria:** Define as dimensões da viga pré-moldada e das camadas da laje/capa.
2.  **2. Materiais e Protensão:** Especifica as propriedades do concreto e os dados das armaduras de protensão.
3.  **3. Ações e Coeficientes:** Informa os momentos fletores, esforços normais externos e os coeficientes de combinação $\psi$.

Utilize os botões "Voltar" e "Próximo" abaixo das abas para navegar entre elas.

### 3.2. Seção de Entradas (Inputs)

#### Aba 1: Geometria

* **1.1 Viga Pré-Moldada:**
    * Tabela para inserção dos elementos geométricos da viga pré-moldada, de baixo para cima.
    * **Elem.:** Número do elemento (gerado automaticamente).
    * **B inf. (m):** Largura inferior do elemento trapezoidal ou retangular.
    * **B sup. (m):** Largura superior do elemento trapezoidal ou retangular.
    * **h (m):** Altura do elemento.
    * **Área (m²):** Área do elemento (calculada automaticamente).
    * Utilize os botões `+` (Adicionar Elemento) e `-` (Remover Último Elemento) para gerenciar as linhas da tabela. No mínimo um elemento é necessário.

* **1.2 Laje/Capa (Composta):**
    * **Laje Pré (1ª Camada):**
        * **Largura (b<sub>f1</sub>) (m):** Largura da primeira camada da laje/capa.
        * **Altura (h<sub>f1</sub>) (m):** Altura da primeira camada da laje/capa.
    * **Capa Conc. (2ª Camada):**
        * **Largura (b<sub>f2</sub>) (m):** Largura da segunda camada da laje/capa.
        * **Altura (h<sub>f2</sub>) (m):** Altura da segunda camada da laje/capa.
    * *Nota: Se não houver laje ou apenas uma camada, preencha com zero as dimensões da camada não existente.*

#### Aba 2: Materiais e Protensão

* **2.1 Concreto:**
    * **f<sub>ck,j</sub> (ATO) (tf/m²):** Resistência característica à compressão do concreto na idade de aplicação da protensão (para verificações no ato da protensão).
    * **f<sub>ck,j</sub> (Serviço) (tf/m²):** Resistência característica à compressão do concreto na idade correspondente às verificações em serviço para cargas de curta duração ou intermediárias (se diferente do fck 28d).
    * **f<sub>ck</sub> (28d) (tf/m²):** Resistência característica à compressão do concreto aos 28 dias (para verificações em serviço de longo prazo).
    * **α (Alfa):** Coeficiente alfa para cálculo da tensão limite de tração no ato da protensão (geralmente 1.0, 1.2 ou 1.5, conforme NBR 6118).

* **2.2 Protensão:**
    * **Protensão - Etapa 1:**
        * **CG Cabos (m):** Distância do centro de gravidade (CG) dos cabos da Etapa 1 à fibra inferior da viga pré-moldada.
        * **Nº Cabos (unid.):** Número de cabos (ou área de aço total multiplicada por uma força unitária de referência) da Etapa 1.
        * **P<sub>o</sub> (tf/cabo):** Força inicial de protensão por cabo (antes das perdas imediatas e progressivas) para a Etapa 1.
        * **P<sub>∞</sub> (tf/cabo):** Força final de protensão por cabo (após todas as perdas) para a Etapa 1.
        * **% P<sub>o</sub> (ATO) (%):** Percentual da força inicial $P_o$ a ser considerado na verificação "Combinação 0 (ATO)". Geralmente 100%.
    * **Protensão - Etapa 2:** (Utilizada para protensão complementar aplicada em uma fase posterior)
        * **CG Cabos (m):** Distância do CG dos cabos da Etapa 2 à fibra inferior da viga pré-moldada.
        * **Nº Cabos (unid.):** Número de cabos da Etapa 2.
        * **P<sub>o</sub> (tf/cabo):** Força inicial de protensão por cabo para a Etapa 2.
        * **P<sub>∞</sub> (tf/cabo):** Força final de protensão por cabo para a Etapa 2.
    * *Nota: Se não houver protensão na Etapa 2, preencha os campos com zero.*

#### Aba 3: Ações e Coeficientes

* **3.1 Momentos Solicitantes (tf.m):**
    * **M<sub>g1</sub>:** Momento fletor devido ao peso próprio da viga pré-moldada.
    * **M<sub>g2</sub>:** Momento fletor devido às cargas permanentes aplicadas após a protensão da Etapa 1 e antes da protensão da Etapa 2 (ex: peso da laje pré-moldada, se aplicável antes da cura).
    * **M<sub>g3</sub>:** Momento fletor devido às demais cargas permanentes (ex: peso da capa de concreto, pavimento, barreiras), aplicadas sobre a seção composta.
    * **M<sub>q</sub>:** Momento fletor devido às cargas acidentais (variáveis).
    * **M<sub>p1o</sub> (calc.), M<sub>p2o</sub> (calc.), M<sub>p1∞</sub> (calc.), M<sub>p2∞</sub> (calc.):** Momentos devidos à excentricidade das forças de protensão (calculados automaticamente e exibidos como referência).

* **3.2 Esforços Normais Solicitantes (tf):**
    * Informe os esforços normais externos (Compressão negativa, Tração positiva) correspondentes a cada parcela de momento.
    * **N<sub>g1</sub>, N<sub>g2</sub>, N<sub>g3</sub>, N<sub>q</sub>:** Esforços normais correspondentes a M<sub>g1</sub>, M<sub>g2</sub>, M<sub>g3</sub>, M<sub>q</sub>.
    * **N<sub>p1o</sub> (calc.), N<sub>p2o</sub> (calc.), N<sub>p1∞</sub> (calc.), N<sub>p2∞</sub> (calc.):** Forças axiais de protensão (calculadas automaticamente, compressão negativa).

* **3.3 Coeficientes (ψ):**
    * **ψ<sub>1</sub>:** Coeficiente de combinação para ações variáveis frequentes (ELS-F).
    * **ψ<sub>2</sub>:** Coeficiente de combinação para ações variáveis quase permanentes (ELS-D).

### 3.3. Painel Lateral

Localizado à direita da seção de entradas.

* **Visualização da Seção:** Apresenta um desenho esquemático da seção transversal da viga e da laje/capa, atualizado dinamicamente conforme os dados de geometria são inseridos. Os CGs das armaduras de protensão (P1 e P2) também são indicados.
* **Observações:** Contém notas importantes sobre o funcionamento da calculadora e convenções adotadas.
* **Botão "Calcular Tensões":** Ao ser clicado, processa todos os dados de entrada e exibe a seção de "Resultados".

### 3.4. Seção de Resultados

Esta seção aparece abaixo do painel de entrada após clicar em "Calcular Tensões".

* **Propriedades Geométricas:**
    * Tabela com as propriedades da seção inicial (viga pré-moldada) e final (composta): Área, Y<sub>inf</sub> (distância do CG à fibra inferior), Y<sub>sup (viga)</sub> (distância do CG à fibra superior da viga), Y<sub>sup (total)</sub> (distância do CG à fibra superior da seção composta), Inércia, W<sub>inf</sub> (módulo resistente inferior), W<sub>sup1</sub> (módulo resistente superior da viga), W<sub>sup2</sub> (módulo resistente superior da seção composta).

* **Tensões Individuais por Ação (tf/m²):**
    * Tabela detalhando as tensões (σ<sub>inf</sub>, σ<sub>sup1</sub>, σ<sub>sup2</sub>) causadas por cada parcela de esforço isoladamente (N<sub>p1o</sub>, M<sub>p1o</sub>, N<sub>p1∞</sub>, M<sub>p1∞</sub>, N<sub>g1</sub>, M<sub>g1</sub>, etc.).
    * *σ<sub>inf</sub>:* Tensão na fibra mais tracionada (ou menos comprimida) da seção.
    * *σ<sub>sup1</sub>:* Tensão na fibra superior da viga pré-moldada.
    * *σ<sub>sup2</sub>:* Tensão na fibra superior da seção composta (laje/capa).

* **Tensões Limites (tf/m²):**
    * Tabela com as tensões limites de compressão e tração para cada fibra (σ<sub>inf</sub>, σ<sub>sup1</sub>, σ<sub>sup2</sub>) e para cada verificação (Combinações 0 a 5), calculadas conforme a NBR 6118.

* **Tensões Calculadas (tf/m²):**
    * Tabela com as tensões resultantes nas fibras (σ<sub>inf</sub>, σ<sub>sup1</sub>, σ<sub>sup2</sub>) para cada combinação de serviço.
    * **Comb.:** Número da combinação:
        * 0: ATO (%P<sub>o</sub> definido pelo usuário) + G1
        * 1: ATO (100% P<sub>o1</sub> + 100% P<sub>o2</sub>) + G1
        * 2: Intermediária (G1 + G2 + P<sub>o1</sub>) - *Verifica tensões na viga antes da cura da capa para P<sub>o2</sub>.*
        * 3: Intermediária (G1 + G2 + P<sub>o1</sub> + P<sub>o2</sub>) - *Verifica tensões na viga (para σ<sub>inf</sub>, σ<sub>sup1</sub>) e na laje (para σ<sub>sup2</sub>) se P<sub>o2</sub> é aplicado após cura da capa.*
        * 4: ELS Frequente (G1+G2+G3 + ψ<sub>1</sub>Q + P<sub>1∞</sub>+P<sub>2∞</sub>)
        * 5: ELS Quase Permanente / Descompressão (G1+G2+G3 + ψ<sub>2</sub>Q + P<sub>1∞</sub>+P<sub>2∞</sub>)
    * **Verif.:** Indicação "OK" (tensão calculada dentro do limite) ou "FALHA" (tensão calculada fora do limite).

* **Momentos de Descompressão (tf.m):**
    * **M<sub>d,inicial</sub> (P<sub>0</sub>, Seção Inicial):** Momento fletor que anula a tensão de compressão na fibra inferior da seção inicial, considerando as forças de protensão iniciais ($P_0$).
    * **M<sub>d,final</sub> (P<sub>∞</sub>, Seção Final):** Momento fletor que anula a tensão de compressão na fibra inferior da seção final (composta), considerando as forças de protensão finais ($P_\infty$).

## 4. Passo a Passo para Utilização

1.  **Aba 1: Geometria**
    * Preencha a tabela da "Viga Pré-Moldada" com as dimensões dos elementos que compõem a viga, começando pelo elemento mais inferior.
    * Insira as dimensões da "Laje Pré (1ª Camada)" e da "Capa Conc. (2ª Camada)". Se alguma camada não existir, deixe suas dimensões como zero.
    * Observe a "Visualização da Seção" sendo atualizada.

2.  **Aba 2: Materiais e Protensão**
    * Insira as resistências do concreto (f<sub>ck,j</sub> ATO, f<sub>ck,j</sub> Serviço, f<sub>ck</sub> 28d) e o coeficiente α.
    * Forneça os dados da "Protensão - Etapa 1": CG dos cabos, número de cabos (ou equivalente), forças $P_o$ e $P_\infty$ por cabo, e o percentual de $P_o$ para a verificação de ATO (Comb. 0).
    * Se aplicável, forneça os dados da "Protensão - Etapa 2". Caso contrário, mantenha os valores como zero.

3.  **Aba 3: Ações e Coeficientes**
    * Insira os momentos fletores (M<sub>g1</sub>, M<sub>g2</sub>, M<sub>g3</sub>, M<sub>q</sub>).
    * Insira os esforços normais externos correspondentes (N<sub>g1</sub>, N<sub>g2</sub>, N<sub>g3</sub>, N<sub>q</sub>), lembrando que compressão é negativa.
    * Verifique os momentos e forças de protensão calculados (campos "calc.").
    * Insira os coeficientes de combinação ψ<sub>1</sub> e ψ<sub>2</sub>.

4.  **Executando o Cálculo**
    * Após preencher todos os dados necessários, clique no botão **"Calcular Tensões"** no painel lateral.

5.  **Interpretando os Resultados**
    * Analise a seção "Resultados" que será exibida.
    * Verifique as "Propriedades Geométricas" calculadas para as seções inicial e final.
    * Examine as "Tensões Individuais por Ação" para entender a contribuição de cada esforço.
    * Compare as "Tensões Calculadas" com as "Tensões Limites" para cada combinação e fibra. A coluna "Verif." indicará "OK" ou "FALHA".
    * Analise os "Momentos de Descompressão" inicial e final.

## 5. Detalhamento dos Cálculos (Principais Conceitos)

A calculadora emprega os seguintes conceitos e formulações básicas:

* **Propriedades Geométricas:** Cálculo de área, centro de gravidade (CG), momento de inércia (I) e módulos de resistência (W) para seções poligonais (viga) e retangulares (lajes), utilizando o teorema dos eixos paralelos para seções compostas.
* **Tensões Normais:**
    * Devido à força axial (N): $\sigma = N/A$
    * Devido ao momento fletor (M): $\sigma = M/W$
* **Tensões Limites (NBR 6118:2023, item 17.2.4.3.2):**
    * **Compressão no ATO:** $\sigma_{cd,ato} \le 0.7 \cdot f_{ckj,ato}$
    * **Tração no ATO (ELS-CP):** $\sigma_{ctd,ato} \le \alpha \cdot f_{ctm,j}$
    * **Compressão em Serviço (ELS):** $\sigma_{cd,serv} \le k_1 \cdot f_{ck}$ (onde $k_1$ varia, ex: 0.60 para ELS-F, 0.45 para ELS-D, 0.70 para intermediárias)
    * **Tração em Serviço (ELS-F):** $\sigma_{ctd,serv} \le f_{ctk,inf}$ (para elementos de concreto protendido do Grupo 1, Classe C P). A calculadora usa $1.2 \cdot f_{ctk,inf}$ como um limite mais geral ou para outras classes, verificar item 17.2.4.3.2 (a) para o limite exato aplicável.
    * **Descompressão (ELS-D):** $\sigma_{ctd,serv} \le 0$ (garantir que a seção esteja comprimida).
* **Tensões Devido à Protensão:**
    * Considera o efeito da força axial e o momento causado pela excentricidade dos cabos.
    * Para $P\infty$, a metodologia de superposição (efeito de P0 na seção inicial + efeito de $\Delta P = P0 - P\infty$ na seção final) é utilizada para as tensões individuais.
* **Combinações de Tensões:** As tensões são somadas algebricamente para cada combinação de serviço especificada.
* **Momentos de Descompressão ($M_d$):**
    * $M_d = (-\sigma_{inf,Protensão}) \cdot W_{inf}$
    * Onde $\sigma_{inf,Protensão}$ é a tensão total de compressão na fibra inferior causada apenas pelas forças de protensão (axiais e de flexão por excentricidade) para a etapa considerada (P0 na seção inicial para $M_{d,inicial}$, e P$\infty$ na seção final para $M_{d,final}$).

## 6. Observações Importantes e Limitações

* A precisão dos resultados depende diretamente da exatidão dos dados de entrada.
* A calculadora não realiza o dimensionamento da armadura passiva, caso necessária.
* Efeitos de segunda ordem, torção, cisalhamento e fadiga não são considerados.
* As perdas de protensão ($P_o \to P_\infty$) devem ser calculadas externamente e fornecidas como input.
* A interpretação dos resultados e a decisão final de engenharia são de responsabilidade do usuário. A ferramenta serve como um auxílio de cálculo.

## 7. Solução de Problemas (Troubleshooting)

* **Resultados "FALHA":** Verifique se as tensões calculadas excedem os limites. Revise os dados de entrada, especialmente as dimensões da seção, propriedades dos materiais, forças de protensão e momentos solicitantes. Pode ser necessário redimensionar a seção ou aumentar a protensão.
* **Valores Inesperados ou "NaN" (Not a Number):**
    * Certifique-se de que todos os campos numéricos obrigatórios foram preenchidos.
    * Evite divisões por zero (ex: altura de elemento igual a zero se ele for usado em cálculos).
    * Verifique se os dados de geometria são consistentes (ex: CG dos cabos dentro da altura da viga).
* **Erro no Desenho da Seção:** Se a visualização da seção apresentar erro, verifique se as dimensões geométricas são válidas e positivas.
* **Console de Erros do Navegador:** Em caso de erros inesperados de script, pressione F12 no navegador para abrir as ferramentas de desenvolvedor e verifique a aba "Console" por mensagens de erro, que podem auxiliar na identificação do problema.

## 8. Sobre o Desenvolvedor

Esta calculadora foi desenvolvida por Leandro Hildinger Cavalcanti, Engenheiro Civil, Especialista em Engenharia de Estruturas.

---

Esperamos que este manual seja útil. Utilize a calculadora com responsabilidade e sempre em conjunto com seu julgamento profissional e conhecimento normativo.
