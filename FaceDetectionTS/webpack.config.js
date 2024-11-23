const ModuleFederationPlugin = require("webpack").container.ModuleFederationPlugin;

module.exports = {
    module: {
        rules: [{
            test: /\.tsx?$/,
            use: 'ts-loader',
            exclude: /node_modules/
        }]
    },
    output: {
        publicPath: "auto",
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
    plugins: [
        new ModuleFederationPlugin({
            name: "FaceDetectionTS",
            filename: "FaceDetectionTS.js",
            exposes: {
                "./FaceDetectionTS": {
                    import : "./src/FaceDetectionTS.ts",
                    name: 'FaceDetectionTS.module'
                }
            },
            shared: {
                "@intuiface/core": {
                    singleton: true,
                    strictVersion: false
                }
            }
        })
    ],
    entry: './src/index.js'
};
