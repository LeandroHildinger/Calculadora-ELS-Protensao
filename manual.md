## Manual de Uso: Calculadora de Tensões em Serviço (ELS) para Elementos Protendidos

**Objetivo:**

Esta calculadora tem como objetivo auxiliar na verificação das tensões em seções de elementos estruturais de concreto protendido (como vigas ou pilares) no Estado Limite de Serviço (ELS), com base nos critérios das normas ABNT NBR 6118 e ABNT NBR 8681.

**Convenção de Sinais:**

* **Tensões:** Compressão é **negativa (-)**, Tração é **positiva (+)**.
* **Momentos Fletores Externos (M<sub>g</sub>, M<sub>q</sub>):** Positivos conforme convenção usual (tracionam fibras inferiores).
* **Esforços Normais Externos (N<sub>g</sub>, N<sub>q</sub>):** Compressão é **negativa (-)**, Tração é **positiva (+)**.

**Unidades:**

* **Entrada:** Utilizar as unidades indicadas nos rótulos dos campos (geralmente tf, m).
* **Resultados:** As tensões (individuais, combinadas e limites) são apresentadas em **tf/m²**. Propriedades geométricas são apresentadas em m, m², m³, m⁴.

**Navegação:**

A calculadora é organizada em abas (ou etapas):

1.  **Geometria:** Definição da seção transversal.
2.  **Materiais e Protensão:** Propriedades do concreto e detalhes da protensão.
3.  **Ações e Coeficientes:** Momentos fletores, esforços normais e coeficientes ψ.

Use as abas no topo ou os botões "‹ Voltar" e "Próximo ›" no final de cada seção para navegar entre as etapas.

**Entrada de Dados:**

**Aba 1: Geometria**

* **1.1 Viga Pré-Moldada:**
    * Defina a geometria da viga principal inserindo as dimensões de cada elemento retangular ou trapezoidal que a compõe, **de baixo para cima**.
    * **Elem.:** Número do elemento (automático).
    * **B inf. (m):** Largura da base inferior do elemento.
    * **B sup. (m):** Largura da base superior do elemento (igual a B inf. para retângulos).
    * **h (m):** Altura do elemento.
    * **Área (m²):** Calculada automaticamente.
    * Use os botões `+` e `-` para adicionar ou remover elementos.
* **1.2 Laje/Capa (Composta):**
    * Insira as dimensões das partes da seção que agem em conjunto com a viga principal (geralmente pré-laje e capa de concreto). Se não houver seção composta, deixe os valores como 0.
    * **Largura (b<sub>f1</sub>), Altura (h<sub>f1</sub>):** Dimensões da Laje Pré (1ª Camada).
    * **Largura (b<sub>f2</sub>), Altura (h<sub>f2</sub>):** Dimensões da Capa Conc. (2ª Camada).

**Aba 2: Materiais e Protensão**

* **2.1 Concreto:**
    * **f<sub>ck,j</sub> (ATO) (tf/m²):** Resistência característica do concreto à compressão na idade `j` da aplicação da protensão (usada para limites de compressão e tração no ATO).
    * **f<sub>ck,j</sub> (Serviço) (tf/m²):** Resistência característica do concreto à compressão na idade `j` (usada para limites de tração em serviço, como $f_{ctm}$). *Nota: Pode ser igual ao fckj(ATO) ou diferente dependendo da idade considerada para as verificações intermediárias.*
    * **f<sub>ck</sub> (28d) (tf/m²):** Resistência característica do concreto à compressão aos 28 dias (usada para limites de compressão em serviço e $f_{ctk,inf}$).
    * **α (Alfa):** Coeficiente que depende da forma da seção transversal e do tipo de protensão, usado para calcular os limites de tração (conforme NBR 6118 item 17.3.1). Insira o valor apropriado:
        * 1.0 para seção retangular.
        * 1.2 para seção T ou duplo T com pré-tração (Default).
        * 1.5 para seção T ou duplo T com pós-tração.
* **2.2 Protensão:**
    * **Etapa 1 e Etapa 2:** Permite definir duas fases de protensão (se a Etapa 2 não for usada, deixe os campos como 0).
    * **CG Cabos (m):** Distância vertical da base inferior da viga até o centro de gravidade dos cabos de protensão.
    * **Nº Cabos:** Número total de cabos/cordoalhas na etapa.
    * **P₀ (tf/cabo):** Força inicial de protensão *por cabo* (antes das perdas).
    * **P<sub class="text-xs">∞</sub> (tf/cabo):** Força final de protensão *por cabo* (após todas as perdas).
    * **% P₀ (ATO):** Percentual da força inicial P₀ a ser considerado na verificação da Combinação 0 (ATO). Geralmente 100%.

**Aba 3: Ações e Coeficientes**

* **3.1 Momentos Solicitantes (tf.m):**
    * **M<sub>g1</sub>, M<sub>g2</sub>, M<sub>g3</sub>:** Momentos fletores devidos às cargas permanentes (G1, G2, G3), aplicados nas fases correspondentes (G1/G2 na seção inicial, G3 na seção final).
    * **M<sub>q</sub>:** Momento fletor devido à carga variável principal (Q).
    * **M<sub>p...</sub> (calc.):** Momentos gerados pela excentricidade da protensão (calculados automaticamente).
* **3.2 Esforços Normais Solicitantes (tf):**
    * **N<sub>g1</sub>, N<sub>g2</sub>, N<sub>g3</sub>:** Esforços normais externos devidos às cargas permanentes (G1, G2, G3).
    * **N<sub>q</sub>:** Esforço normal externo devido à carga variável principal (Q).
    * **N<sub>p...</sub> (calc.):** Forças normais de protensão (calculadas automaticamente, compressão é negativa).
* **3.3 Coeficientes (ψ):**
    * **ψ<sub class="text-xs">1</sub>:** Fator de combinação frequente para a carga variável principal (Mq, Nq).
    * **ψ<sub class="text-xs">2</sub>:** Fator de combinação quase permanente para a carga variável principal (Mq, Nq).

**Botão "Calcular Tensões":**

* Localizado abaixo das Observações.
* Clique neste botão após preencher todos os dados de entrada para gerar os resultados.

**Resultados:**

A seção de resultados é exibida após o cálculo e contém:

* **Propriedades Geométricas:** Área, posição do CG (Yinf, Ysup), Inércia e Módulos Resistentes (Winf, Wsup1, Wsup2) para a seção inicial (viga) e final (composta).
* **Tensões Individuais por Ação (tf/m²):** Mostra a contribuição de cada ação (momentos Mg, Mq; esforços normais Ng, Nq; protensão P0, Pinf - axial e flexão) para as tensões nas fibras inferior (σ<sub>inf</sub>), superior da viga (σ<sub>sup1</sub>) e superior da capa (σ<sub>sup2</sub>).
* **Tensões Limites (tf/m²):** Apresenta os valores limites de tensão (compressão e tração) para cada fibra relevante em cada uma das 6 verificações (0 a 5), calculados com base no fck e α informados e nas fórmulas da NBR 6118.
    * **Verif. 0 (ATO %P₀):** σ<sub>inf</sub> ≥ -0.7⋅f<sub>ck,j(ATO)</sub> | σ<sub>sup1</sub> ≤ α⋅f<sub>ctm,j(ATO)</sub>
    * **Verif. 1 (ATO 100%):** σ<sub>inf</sub> ≥ -0.7⋅f<sub>ck,j(ATO)</sub> | σ<sub>sup1</sub> ≤ α⋅f<sub>ctm,j(ATO)</sub>
    * **Verif. 2 (Interm. G1+G2+P1o):** σ<sub>inf</sub> ≤ f<sub>ctm,j(Serv)</sub> | σ<sub>sup1</sub> ≥ -0.7⋅f<sub>ck</sub>
    * **Verif. 3 (Interm. G1+G2+P1o+P2o):** σ<sub>inf</sub> ≥ -0.7⋅f<sub>ck</sub> | σ<sub>sup2</sub> ≤ f<sub>ctm,j(Serv)</sub>
    * **Verif. 4 (ELS-F):** σ<sub>inf</sub> ≤ f<sub>ctm,j(Serv)</sub> | σ<sub>sup2</sub> ≥ -0.6⋅f<sub>ck</sub>
    * **Verif. 5 (ELS-D):** σ<sub>inf</sub> ≤ 0 | σ<sub>sup2</sub> ≥ -0.45⋅f<sub>ck</sub>
* **Tensões Calculadas (tf/m²):** Mostra as tensões finais combinadas para cada uma das 6 verificações (0 a 5), somando as tensões individuais relevantes.
    * **Comb. 0:** σ<sub>Mg1</sub> + σ<sub>Ng1</sub> + (%P₀/100)⋅1.1⋅(σ<sub>P1o(ax)</sub>+σ<sub>P1o(flex)</sub>)
    * **Comb. 1:** σ<sub>Mg1</sub> + σ<sub>Ng1</sub> + 1.1⋅(σ<sub>P1o(ax)</sub>+σ<sub>P1o(flex)</sub>)
    * **Comb. 2:** σ<sub>Mg1</sub>+σ<sub>Mg2</sub>+σ<sub>Ng1</sub>+σ<sub>Ng2</sub>+σ<sub>P1o(ax)</sub>+σ<sub>P1o(flex)</sub>
    * **Comb. 3:** σ<sub>Mg1</sub>+σ<sub>Mg2</sub>+σ<sub>Ng1</sub>+σ<sub>Ng2</sub>+σ<sub>P1o(ax)</sub>+σ<sub>P1o(flex)</sub>+σ<sub>P2o(ax)</sub>+σ<sub>P2o(flex)</sub>
    * **Comb. 4:** Σσ<sub>G</sub>+Σσ<sub>N,G</sub>+σ<sub>P</sub>+ψ<sub>1</sub>⋅(σ<sub>Mq</sub>+σ<sub>Nq</sub>)  *(σ<sub>P</sub> usa P<sub>0</sub> para σ<sub>sup1</sub> e P<sub>∞</sub> para σ<sub>inf</sub>, σ<sub>sup2</sub>)*
    * **Comb. 5:** Σσ<sub>G</sub>+Σσ<sub>N,G</sub>+σ<sub>P</sub>+ψ<sub>2</sub>⋅(σ<sub>Mq</sub>+σ<sub>Nq</sub>)  *(σ<sub>P</sub> usa P<sub>0</sub> para σ<sub>sup1</sub> e P<sub>∞</sub> para σ<sub>inf</sub>, σ<sub>sup2</sub>)*
    * **Verif.:** Indica se a seção passa ("OK" em verde) ou falha ("FALHA" em vermelho) na verificação, comparando as tensões calculadas com os limites.

**Visualização da Seção:**

* Um desenho esquemático da seção transversal definida é exibido, com os elementos da viga numerados e a posição dos CGs da protensão indicados.

**Observações:**

* Lembre-se de inserir os elementos da viga de baixo para cima na tabela de geometria.
* Verifique as convenções de sinal e as unidades ao inserir os dados.
* Os critérios de combinação e limites seguem as premissas da NBR 6118.

