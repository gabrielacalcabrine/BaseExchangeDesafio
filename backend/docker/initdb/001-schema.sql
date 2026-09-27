-- Schema inicial do OrderAccumulator. O PostgreSQL executa este arquivo na primeira criação do volume.
CREATE TABLE IF NOT EXISTS ativos (
    codigo VARCHAR(10) PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    usuario_inclusao VARCHAR(150) NOT NULL,
    usuario_alteracao VARCHAR(150),
    data_inclusao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ
);

INSERT INTO ativos (codigo, nome, usuario_inclusao)
VALUES
    ('PETR4', 'Petrobras', 'system'),
    ('VALE3', 'Vale', 'system'),
    ('VIIA4', 'Via', 'system')
ON CONFLICT (codigo) DO NOTHING;

CREATE TABLE IF NOT EXISTS ordens (
    id BIGSERIAL PRIMARY KEY,
    ativo_codigo VARCHAR(10) NOT NULL REFERENCES ativos(codigo),
    lado CHAR(1) NOT NULL CHECK (lado IN ('C', 'V')),
    quantidade INTEGER NOT NULL CHECK (quantidade > 0 AND quantidade < 100000),
    preco NUMERIC(18, 2) NOT NULL CHECK (preco > 0 AND preco < 1000),
    valor_financeiro NUMERIC(18, 2) NOT NULL,
    usuario_inclusao VARCHAR(150) NOT NULL,
    usuario_alteracao VARCHAR(150),
    data_inclusao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS exposicoes (
    ativo_codigo VARCHAR(10) PRIMARY KEY REFERENCES ativos(codigo),
    valor_atual NUMERIC(18, 2) NOT NULL DEFAULT 0,
    usuario_inclusao VARCHAR(150) NOT NULL,
    usuario_alteracao VARCHAR(150),
    data_inclusao TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    data_alteracao TIMESTAMPTZ,
    versao BIGINT NOT NULL DEFAULT 1
);

INSERT INTO exposicoes (ativo_codigo, usuario_inclusao)
SELECT codigo, 'system' FROM ativos
ON CONFLICT (ativo_codigo) DO NOTHING;
