﻿/********************************************************************************/
/* Projeto: Biblioteca ZeusDFe                                                  */
/* Biblioteca C# para auxiliar no desenvolvimento das demais bibliotecas DFe    */
/*                                                                              */
/*                                                                              */
/* Direitos Autorais Reservados (c) 2014 Adenilton Batista da Silva             */
/*                                       Zeusdev Tecnologia LTDA ME             */
/*                                                                              */
/*  Você pode obter a última versão desse arquivo no GitHub                     */
/* localizado em https://github.com/adeniltonbs/Zeus.Net.NFe.NFCe               */
/*                                                                              */
/*                                                                              */
/*  Esta biblioteca é software livre; você pode redistribuí-la e/ou modificá-la */
/* sob os termos da Licença Pública Geral Menor do GNU conforme publicada pela  */
/* Free Software Foundation; tanto a versão 2.1 da Licença, ou (a seu critério) */
/* qualquer versão posterior.                                                   */
/*                                                                              */
/*  Esta biblioteca é distribuída na expectativa de que seja útil, porém, SEM   */
/* NENHUMA GARANTIA; nem mesmo a garantia implícita de COMERCIABILIDADE OU      */
/* ADEQUAÇÃO A UMA FINALIDADE ESPECÍFICA. Consulte a Licença Pública Geral Menor*/
/* do GNU para mais detalhes. (Arquivo LICENÇA.TXT ou LICENSE.TXT)              */
/*                                                                              */
/*  Você deve ter recebido uma cópia da Licença Pública Geral Menor do GNU junto*/
/* com esta biblioteca; se não, escreva para a Free Software Foundation, Inc.,  */
/* no endereço 59 Temple Street, Suite 330, Boston, MA 02111-1307 USA.          */
/* Você também pode obter uma copia da licença em:                              */
/* http://www.opensource.org/licenses/lgpl-license.php                          */
/*                                                                              */
/* Zeusdev Tecnologia LTDA ME - adenilton@zeusautomacao.com.br                  */
/* http://www.zeusautomacao.com.br/                                             */
/* Rua Comendador Francisco josé da Cunha, 111 - Itabaiana - SE - 49500-000     */
/********************************************************************************/
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using DFe.Utils.Assinatura.CertificadoProvider.Contratos;

namespace DFe.Utils.Assinatura.CertificadoProvider
{
    public class CertificadoProvider : ICertificadoProvider
    {
        public X509Certificate2 Provider(string numeroSerial, string senha = null)
        {
            var certificado = X509StoreHelper.ObterPeloSerial(numeroSerial, OpenFlags.MaxAllowed);

            if (string.IsNullOrEmpty(senha)) return certificado;

            //Se a senha for passada no parâmetro
            var senhaSegura = new SecureString();
            var passPhrase = senha.ToCharArray();
            foreach (var t in passPhrase)
            {
                senhaSegura.AppendChar(t);
            }

            var chavePrivada = certificado.PrivateKey as RSACryptoServiceProvider;
            if (chavePrivada == null) return certificado;

            var cspParameters = new CspParameters(chavePrivada.CspKeyContainerInfo.ProviderType,
                chavePrivada.CspKeyContainerInfo.ProviderName,
                chavePrivada.CspKeyContainerInfo.KeyContainerName,
                null,
                senhaSegura);
            var rsaCsp = new RSACryptoServiceProvider(cspParameters);
            certificado.PrivateKey = rsaCsp;

            return certificado;
        }
    }
}