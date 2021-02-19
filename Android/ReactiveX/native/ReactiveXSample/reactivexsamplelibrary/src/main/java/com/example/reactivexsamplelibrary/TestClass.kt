package com.example.reactivexsamplelibrary

import android.content.Context
import android.widget.TextView
import io.reactivex.Observable
import io.reactivex.Observer
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.disposables.Disposable
import io.reactivex.rxkotlin.subscribeBy
import io.reactivex.rxkotlin.toObservable
import io.reactivex.schedulers.Schedulers

class TestClass {
    private fun getFoodsObservable(): Observable<String> {
        return Observable.just("Apple", "Bacon", "Cacao", "Dumpling", "Fish", "Milk")
    }

    private fun getFoodsObserver(tv: TextView): Observer<String> {
        return object : Observer<String> {
            override fun onSubscribe(d: Disposable) {
                tv.append("onSubscribe=$d\n")
            }

            override fun onNext(t: String) {
                tv.append("onNext=$t\n")
            }

            override fun onError(e: Throwable) {
                tv.append("onError=${e.message}\n")
            }

            override fun onComplete() {
                tv.append("onComplete\n\n")
            }
        }
    }

    private fun getAnyObservable(): Observable<Any> {
        return listOf(true, 1, 2, "Three", 4.0f, 4.5, "Five", false).toObservable()
    }

    fun testRxKotlin(tv: TextView) {
        val anyObservable = getAnyObservable()
        val disposable = anyObservable
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
            .subscribeBy(
                onNext = {
                    tv.append("testRxKotlin-onNext=$it\n")
                },
                onError = {
                    tv.append("testRxKotlin-onError=${it.message}\n")
                },
                onComplete = {
                    tv.append("testRxKotlin-onComplete\n\n")
                }
            )
    }

    fun testReactiveX(tv: TextView) {
        val foodsObservable = getFoodsObservable()
        val foodsObserver = getFoodsObserver(tv)

        foodsObservable
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
            .subscribe(foodsObserver)
    }
}
