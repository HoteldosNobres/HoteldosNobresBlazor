using System;
using System.ComponentModel;

namespace HoteldosNobresBlazor.Classes;

public enum SNRHosException
{
    [Description("SNRHos-ME0001 – Tipo de dado inválido.")]
    TipoDadoInvalido = 1,

    [Description("SNRHos-ME0002 – Identificador de domínio não localizado.")]
    IdentificadorDominioNaoLocalizado,

    [Description("SNRHos-ME0003 – Identificador de território não localizado.")]
    IdentificadorTerritorioNaoLocalizado,

    [Description("SNRHos-ME0004 – Identificador de meio de hospedagem não localizado.")]
    IdentificadorMeioHospedagemNaoLocalizado,

    [Description("SNRHos-ME0005 – Chave de acesso não localizado.")]
    ChaveAcessoNaoLocalizado,

    [Description("SNRHos-ME0006 – Identificador de FNRH não encontrado.")]
    IdentificadorFNRHNaoEncontrado,

    [Description("SNRHos-ME0007 – Identificador número da FNRH não encontrado.")]
    IdentificadorNumeroFNRHNaoEncontrado,

    [Description("SNRHos-ME0008 - Identificador número da FNRH status não encontrado.")]
    IdentificadorNumeroFNRHStatusNaoEncontrado,

    [Description("SNRHos-ME0009 – Identificador status não encontrado.")]
    IdentificadorStatusNaoEncontrado,

    [Description("SNRHos-ME0010 - Documento do hóspede inexistente.")]
    DocumentoHospedeInexistente,

    [Description("SNRHos-ME0011 – Atributo obrigatório não encontrado.")]
    AtributoObrigatorioNaoEncontrado,

    [Description("SNRHos-ME0012 – País não encontrado.")]
    PaisNaoEncontrado,

    [Description("SNRHos-ME0013 – UF não encontrada.")]
    UFNaoEncontrada,

    [Description("SNRHos-ME0014 – UF inválida.")]
    UFInvalida,

    [Description("SNRHos-ME0015 – Cidade não encontrada.")]
    CidadeNaoEncontrada,

    [Description("SNRHos-ME0016 – Cidade inválida.")]
    CidadeInvalida,

    [Description("SNRHos-ME0017 – Motivo de viagem não encontrado.")]
    MotivoViagemNaoEncontrado,

    [Description("SNRHos-ME0018 – Tipo de transporte não encontrado.")]
    TipoTransporteNaoEncontrado,

    [Description("SNRHos-ME0019 – Chave de acesso inativa.")]
    ChaveAcessoInativa,

    [Description("SNRHos-ME0020 – FNRH pertencente a outro meio de hospedagem.")]
    FNRHPertencenteOutroMeioHospedagem,

    [Description("SNRHos-ME0021 – Identificador motivo de reserva não encontrado.")]
    IdentificadorMotivoReservaNaoEncontrado,

    [Description("SNRHos-ME0022 – Data de checkin inválida.")]
    DataCheckinInvalida,

    [Description("SNRHos-ME0023 - Data de checkout inválida.")]
    DataCheckoutInvalida,

    [Description("SNRHos-ME0024 – Checkin não permitido.")]
    CheckinNaoPermitido,

    [Description("SNRHos-ME0025 – Checkout não permitido.")]
    CheckoutNaoPermitido,

    [Description("SNRHos-ME0026 – CPF inválido.")]
    CPFInvalido,

    [Description("SNRHos-ME0027 – CPF ausente.")]
    CPFAusente,

    [Description("SNRHos-ME0028 – Certidão de nascimento ausente.")]
    CertidaoNascimentoAusente,

    [Description("SNRHos-ME0029 – Hóspede pertencente ao MERCOSUL com passaporte ausente.")]
    HospedeMERCOSULPassaporteAusente,

    [Description("SNRHos-ME0030 - Hóspede pertencente ao MERCOSUL com carteira de identidade estrangeira ausente.")]
    HospedeMERCOSULCarteiraIdentidadeEstrangeiraAusente,

    [Description("SNRHos-ME0031 – Atualização de registro da FNRH não permitido.")]
    AtualizacaoRegistroFNRHNaoPermitido
}
 