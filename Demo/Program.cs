using Algorithms.Common;
using Algorithms.DSA;
using Algorithms.RSA;
using Demo;
using Modelling.CustomTransformers;

var randomProvider = new RandomProvider();

var objectToByteArrayTransformer = new ObjectToByteArrayTransformer();
objectToByteArrayTransformer.TypeTransformers.Add(new GuidTransformer());
objectToByteArrayTransformer.TypeTransformers.Add(new ModellingTransformer());

var encryptionProvider = new RSAEncryptionProvider();
var encryptionKeyGenerator = new RSAKeysGenerator();

var signatureProvider = new DSASignatureProvider();
var signatureKeyGenerator = new DSAKeysGenerator();

var dataFactory = new DemoDataFactory(encryptionProvider, encryptionKeyGenerator, signatureProvider, signatureKeyGenerator, objectToByteArrayTransformer, randomProvider);
var candidates = dataFactory.CreateCandidates();
var voters = dataFactory.CreateVoters();
var centralElectionCommission = dataFactory.CreateCentralElectionCommission();

var printer = new ModellingPrinter(dataFactory);

printer.PrintUsualVoting(centralElectionCommission, voters, candidates);
