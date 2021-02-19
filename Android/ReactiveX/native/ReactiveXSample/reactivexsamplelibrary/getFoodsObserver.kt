private fun getFoodsObserver(): Observer<String> {
    return object : Observer<String> {
        override fun onSubscribe(d: Disposable) {
            Log.i("onSubscribe", d.toString())
        }

        override fun onNext(t: String) {
            Log.i("onNext", t)
        }

        override fun onError(e: Throwable) {
            Log.i("onError", e.toString())
        }

        override fun onComplete() {
            Log.i("onComplete", "DONE!!")
        }
    }
}